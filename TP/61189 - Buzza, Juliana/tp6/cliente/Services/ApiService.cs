using System.Net.Http.Json;
using cliente.Models; 
using cliente.Services; 
using System; 

namespace cliente.Services 
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly CartStateService _cartStateService; 
        private int? _currentCartId; 

        public ApiService(HttpClient httpClient, CartStateService cartStateService)
        {
            _httpClient = httpClient;
            _cartStateService = cartStateService;
        }

        public async Task<List<Producto>> GetProductos(string? query = null)
        {
            var url = "/productos";
            if (!string.IsNullOrEmpty(query))
            {
                url += $"?query={query}";
            }
            return await _httpClient.GetFromJsonAsync<List<Producto>>(url) ?? new List<Producto>();
        }

        public async Task<int> InitializeCart()
        {
            var response = await _httpClient.PostAsync("/carritos", null);
            response.EnsureSuccessStatusCode();
            var newCart = await response.Content.ReadFromJsonAsync<Compra>();
            if (newCart != null)
            {
                _currentCartId = newCart.Id;
                // No establecer CartId aquí, se hará desde el componente
                return newCart.Id;
            }
            throw new Exception("No se pudo inicializar el carrito.");
        }

        public async Task<List<CarritoItemDto>> GetCartItems(int cartId)
        {
            return await _httpClient.GetFromJsonAsync<List<CarritoItemDto>>($"/carritos/{cartId}") ?? new List<CarritoItemDto>();
        }

        /// <summary>
        /// Agrega o actualiza un producto en el carrito.
        /// Devuelve el mensaje de error del servidor si la operación falla (por ejemplo, por stock).
        /// </summary>
        /// <param name="cartId">ID del carrito.</param>
        /// <param name="productId">ID del producto.</param>
        /// <param name="quantity">Cantidad deseada del producto en el carrito.</param>
        /// <returns>Null si la operación fue exitosa; un string con el mensaje de error si falló.</returns>
        public async Task<string?> AddOrUpdateProductInCart(int cartId, int productId, int quantity)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/carritos/{cartId}/{productId}", quantity);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al agregar/actualizar producto: {errorMessage}");
                    return errorMessage;
                }

                await _cartStateService.UpdateCartItemCount(cartId); 
                return null; 
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de conexión al servidor: {ex.Message}");
                return $"Error de conexión: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en AddOrUpdateProductInCart: {ex.Message}");
                return $"Error inesperado: {ex.Message}";
            }
        }

        public async Task RemoveProductFromCart(int cartId, int productId)
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{cartId}/{productId}");
            response.EnsureSuccessStatusCode();
            await _cartStateService.UpdateCartItemCount(cartId); 
        }

        public async Task ClearCart(int cartId)
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{cartId}/vaciar");
            response.EnsureSuccessStatusCode();
            await _cartStateService.UpdateCartItemCount(cartId); 
        }

        /// <summary>
        /// Confirma una compra.
        /// Devuelve el objeto Compra confirmada si es exitosa; un string con el mensaje de error si falla.
        /// </summary>
        /// <param name="cartId">ID del carrito a confirmar.</param>
        /// <param name="confirmationData">Datos de confirmación del cliente.</param>
        /// <returns>Una tupla donde Item1 es la Compra confirmada (o null si hay error) y Item2 es el mensaje de error (o null si es exitoso).</returns>
        public async Task<Tuple<Compra?, string?>> ConfirmPurchase(int cartId, CompraConfirmationDto confirmationData)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/carritos/{cartId}/confirmar", confirmationData);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al confirmar compra: {errorMessage}");
                    return Tuple.Create<Compra?, string?>(null, errorMessage);
                }

                var confirmedCompra = await response.Content.ReadFromJsonAsync<Compra>();
                // No limpiar el CartId aquí, se hará desde el componente
                return Tuple.Create<Compra?, string?>(confirmedCompra, null);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de conexión al confirmar compra: {ex.Message}");
                return Tuple.Create<Compra?, string?>(null, $"Error de conexión: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en ConfirmPurchase: {ex.Message}");
                return Tuple.Create<Compra?, string?>(null, $"Error inesperado: {ex.Message}");
            }
        }
    }
}
