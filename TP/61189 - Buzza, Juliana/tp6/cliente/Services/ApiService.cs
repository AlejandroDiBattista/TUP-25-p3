using System.Net.Http.Json;
using cliente.Models; 
using cliente.Services; 

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
                _cartStateService.SetCartId(newCart.Id);
                return newCart.Id;
            }
            throw new Exception("No se pudo inicializar el carrito.");
        }

        public async Task<List<CarritoItemDto>> GetCartItems(int cartId)
        {
            return await _httpClient.GetFromJsonAsync<List<CarritoItemDto>>($"/carritos/{cartId}") ?? new List<CarritoItemDto>();
        }

        public async Task AddOrUpdateProductInCart(int cartId, int productId, int quantity)
        {
            var response = await _httpClient.PutAsJsonAsync($"/carritos/{cartId}/{productId}", quantity);
            response.EnsureSuccessStatusCode();
            await _cartStateService.UpdateCartItemCount(cartId); 
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

        public async Task<Compra> ConfirmPurchase(int cartId, CompraConfirmationDto confirmationData)
        {
            var response = await _httpClient.PutAsJsonAsync($"/carritos/{cartId}/confirmar", confirmationData);
            response.EnsureSuccessStatusCode();
            _cartStateService.SetCartId(null); 
            return await response.Content.ReadFromJsonAsync<Compra>() ?? throw new Exception("Error al confirmar la compra.");
        }
    }
}
