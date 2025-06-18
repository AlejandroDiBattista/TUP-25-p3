// Services/ApiService.cs
using System.Net.Http.Json;
using cliente.Models;
using cliente.DTOs;

namespace cliente.Services
{
    // Servicio para interactuar con la API del backend.
    public class ApiService
    {
        private readonly HttpClient _httpClient; // Cliente HTTP para realizar peticiones
        private readonly CartStateService _cartStateService; // Servicio para gestionar el estado del carrito global

        // Constructor que recibe HttpClient y CartStateService inyectados.
        public ApiService(HttpClient httpClient, CartStateService cartStateService)
        {
            _httpClient = httpClient;
            _cartStateService = cartStateService;
        }

        // --- Endpoints de Productos ---

        // Obtiene todos los artículos del inventario.
        public async Task<List<ArticuloInventario>?> GetProductosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("productos");
                response.EnsureSuccessStatusCode(); // Lanza excepción si el código de estado no es de éxito (2xx)
                return await response.Content.ReadFromJsonAsync<List<ArticuloInventario>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                // Puedes implementar una lógica de reintento o mostrar un mensaje al usuario.
                return null;
            }
        }

        // Busca artículos por texto.
        public async Task<List<ArticuloInventario>?> SearchProductosAsync(string searchText)
        {
            try
            {
                var response = await _httpClient.GetAsync($"productos/buscar/{searchText}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ArticuloInventario>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al buscar productos: {ex.Message}");
                return null;
            }
        }

        // --- Endpoints de Carrito ---

        // Crea un nuevo carrito y obtiene su ID.
        public async Task<string?> CreateCarritoAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("carritos", null); // Petición POST sin cuerpo
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (result != null && result.TryGetValue("identificadorNuevoCarrito", out var cartId))
                {
                    _cartStateService.SetCartId(cartId); // Actualiza el servicio de estado del carrito
                    return cartId;
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al crear carrito: {ex.Message}");
                return null;
            }
        }

        // Obtiene los ítems de un carrito por su ID.
        public async Task<List<DetalleCarritoMemoria>?> GetCarritoItemsAsync(string cartId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"carritos/{cartId}");
                response.EnsureSuccessStatusCode();
                // El backend devuelve una lista de objetos anónimos, mapeamos a DetalleCarritoMemoria
                // Para simplificar, el backend devuelve el tipo 'object' con las propiedades, así que mapeamos directamente
                return await response.Content.ReadFromJsonAsync<List<DetalleCarritoMemoria>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener ítems del carrito: {ex.Message}");
                return null;
            }
        }

        // Añade o actualiza un artículo en el carrito.
        public async Task<bool> AddItemToCarritoAsync(string cartId, int articuloId, int cantidad)
        {
            try
            {
                var response = await _httpClient.PutAsync($"carritos/{cartId}/anadir/{articuloId}?cantidadSolicitada={cantidad}", null);
                var content = await response.Content.ReadAsStringAsync(); // Leer el contenido para depurar errores
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al añadir ítem al carrito: {response.StatusCode} - {content}");
                    // Aquí puedes parsear el JSON de error del backend si existe y mostrarlo al usuario
                    var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (errorResponse != null && errorResponse.TryGetValue("error", out var errorMessage))
                    {
                        Console.WriteLine($"Mensaje de error del backend: {errorMessage}");
                        // Propagar el mensaje de error o manejarlo en la UI
                        throw new Exception(errorMessage); // Lanza una excepción con el mensaje del backend
                    }
                    throw new HttpRequestException($"La petición no fue exitosa: {response.StatusCode}");
                }
                
                _cartStateService.SetCartItems(await response.Content.ReadFromJsonAsync<List<DetalleCarritoMemoria>>()); // Actualiza el estado
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de red al añadir ítem: {ex.Message}");
                return false;
            }
            catch (Exception ex) // Captura la excepción lanzada desde el backend
            {
                Console.WriteLine($"Error lógico al añadir ítem: {ex.Message}");
                return false;
            }
        }
        
        // Elimina o reduce la cantidad de un artículo en el carrito.
        public async Task<bool> RemoveItemFromCarritoAsync(string cartId, int articuloId, int cantidad = 0)
        {
            try
            {
                string url = $"carritos/{cartId}/remover/{articuloId}";
                if (cantidad > 0)
                {
                    url += $"?cantidadAReducir={cantidad}";
                }
                var response = await _httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                _cartStateService.SetCartItems(await response.Content.ReadFromJsonAsync<List<DetalleCarritoMemoria>>());
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al remover ítem del carrito: {ex.Message}");
                return false;
            }
        }

        // Vacía el carrito por completo.
        public async Task<bool> ClearCarritoAsync(string cartId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"carritos/{cartId}");
                response.EnsureSuccessStatusCode();
                _cartStateService.SetCartItems(new List<DetalleCarritoMemoria>()); // Vacía el estado local del carrito
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al vaciar carrito: {ex.Message}");
                return false;
            }
        }

        // --- Endpoint de Compra ---

        // Confirma la compra enviando los datos del cliente.
        public async Task<bool> ConfirmCompraAsync(string cartId, DatosClienteDTO clientData)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"carritos/{cartId}/confirmar", clientData);
                var content = await response.Content.ReadAsStringAsync(); // Leer el contenido para depurar errores
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al confirmar compra: {response.StatusCode} - {content}");
                    var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (errorResponse != null && errorResponse.TryGetValue("error", out var errorMessage))
                    {
                        Console.WriteLine($"Mensaje de error del backend: {errorMessage}");
                        throw new Exception(errorMessage); // Propaga el mensaje de error del backend
                    }
                    throw new HttpRequestException($"La petición de confirmación no fue exitosa: {response.StatusCode}");
                }
                
                _cartStateService.SetCartId(null); // Limpia el ID del carrito después de la confirmación
                _cartStateService.SetCartItems(new List<DetalleCarritoMemoria>()); // Vacía los ítems
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de red al confirmar compra: {ex.Message}");
                return false;
            }
            catch (Exception ex) // Captura la excepción lanzada desde el backend
            {
                Console.WriteLine($"Error lógico al confirmar compra: {ex.Message}");
                return false;
            }
        }
    }
}
