// --------------------------------------------------------------------------------------
// ApiService.cs - Servicio para consumir la API desde el cliente Blazor WASM
// Archivo exhaustivamente comentado línea por línea y por bloque.
// --------------------------------------------------------------------------------------

using System.Net.Http.Json; // Para métodos de extensión HTTP y JSON
using cliente.Dtos; // Para los DTOs compartidos

namespace cliente.Services
{
    // Servicio que centraliza todas las llamadas HTTP a la API del backend
    public class ApiService
    {
        private readonly HttpClient _httpClient; // Cliente HTTP inyectado
        private readonly CarritoService _carritoService;
        private int carritoId = 0; // ID del carrito actual (persistente en la sesión)

        // Constructor: recibe el HttpClient configurado por DI
        public ApiService(HttpClient httpClient, CarritoService carritoService)
        {
            _httpClient = httpClient;
            _carritoService = carritoService;
        }

        // Obtiene la lista de productos, opcionalmente filtrando por búsqueda
        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
        {
            try
            {
                string url = "/productos";
                if (!string.IsNullOrWhiteSpace(busqueda))
                    url += $"?q={System.Net.WebUtility.UrlEncode(busqueda)}"; // Agrega query si hay búsqueda
                var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url); // Llama a la API
                return productos ?? new List<Producto>(); // Devuelve lista vacía si es null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        // Obtiene el ID del carrito actual o crea uno nuevo si no existe
        public async Task<int> GetOrCreateCarritoIdAsync()
        {
            if (carritoId == 0)
            {
                var response = await _httpClient.PostAsync("/carritos", null); // Crea carrito
                if (response.IsSuccessStatusCode)
                {
                    var idString = await response.Content.ReadAsStringAsync();
                    if (int.TryParse(idString, out int id))
                        carritoId = id; // Si la respuesta es solo el ID
                    else
                    {
                        try
                        {
                            var obj = System.Text.Json.JsonDocument.Parse(idString);
                            if (obj.RootElement.TryGetProperty("value", out var val))
                                carritoId = val.GetInt32(); // Si la respuesta es un objeto JSON
                        }
                        catch { }
                    }
                }
            }
            return carritoId;
        }

        // Agrega un producto al carrito (cantidad=1 por defecto)
        public async Task<bool> AgregarProductoAlCarritoAsync(Producto producto)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.PutAsync($"/carritos/{id}/{producto.Id}?cantidad=1", null);
            return response.IsSuccessStatusCode;
        }

        // Cambia la cantidad de un producto en el carrito
        public async Task<bool> CambiarCantidadProductoAsync(int productoId, int cantidad)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.PutAsync($"/carritos/{id}/{productoId}?cantidad={cantidad}", null);
            return response.IsSuccessStatusCode;
        }

        // Elimina un producto del carrito
        public async Task<bool> EliminarProductoDelCarritoAsync(int productoId)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.DeleteAsync($"/carritos/{id}/{productoId}");
            return response.IsSuccessStatusCode;
        }

        // Vacía el carrito eliminando todos los ítems
        public async Task<bool> VaciarCarritoAsync()
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.DeleteAsync($"/carritos/{id}");
            return response.IsSuccessStatusCode;
        }

        // Confirma la compra enviando los datos del cliente
        public async Task<(bool exito, string mensaje)> ConfirmarCompraAsync(object cliente)
        {
            int id = await GetOrCreateCarritoIdAsync();

            // Validar datos mínimos
            if (cliente == null)
                return (false, "Datos del cliente incompletos");
            if (_carritoService.Items == null || !_carritoService.Items.Any())
                return (false, "El carrito está vacío");

            // Construir el objeto con cliente e items
            var compra = new
            {
                Cliente = cliente,
                Items = _carritoService.Items.Select(i => new { ProductoId = i.Producto.Id, Cantidad = i.Cantidad }).ToList()
            };

            var response = await _httpClient.PutAsJsonAsync($"/carritos/{id}/confirmar", compra);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                // Mostrar el error real del backend si existe
                return (false, $"Error al confirmar la compra: {json}");
            }
            if (string.IsNullOrWhiteSpace(json))
                return (true, "Compra confirmada correctamente");
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("mensaje", out var msgProp))
                {
                    return (true, msgProp.GetString() ?? "Compra confirmada correctamente");
                }
            }
            catch { }
           
            return (true, "Compra confirmada correctamente");
        }

        // Resetea el ID del carrito (por ejemplo, tras confirmar compra)
        public async Task ResetCarritoIdAsync()
        {
            carritoId = 0;
        }
    }

    // Modelo de producto usado en el cliente (debe coincidir con el backend)
    public class Producto
    {
        public int Id { get; set; } // Identificador único
        public string Nombre { get; set; } // Nombre del producto
        public string Descripcion { get; set; } // Descripción
        public decimal Precio { get; set; } // Precio unitario
        public int Stock { get; set; } // Stock disponible
        public string ImagenUrl { get; set; } // URL de la imagen
    }
}
