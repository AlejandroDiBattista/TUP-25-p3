using System.Net.Http.Json;
using cliente.Models; 

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
            _httpClient.BaseAddress = new Uri("https://localhost:7001/");
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
        public class CartStateService
        {
            private readonly HttpClient _httpClient; 

            public event Action? OnCartItemCountChanged;
            public event Action? OnCartIdChanged;

            private int _itemCount = 0;
            private int? _cartId = null;

            public CartStateService(HttpClient httpClient) 
            {
                _httpClient = httpClient;
            }

            public int ItemCount
            {
                get => _itemCount;
                private set
                {
                    if (_itemCount != value)
                    {
                        _itemCount = value;
                        OnCartItemCountChanged?.Invoke();
                    }
                }
            }

            public int? CartId
            {
                get => _cartId;
                private set
                {
                    if (_cartId != value)
                    {
                        _cartId = value;
                        OnCartIdChanged?.Invoke();
                    }
                }
            }

            public void SetCartId(int? id)
            {
                CartId = id;
                if (id == null)
                {
                    ItemCount = 0; 
                }
            }

            public async Task UpdateCartItemCount(int cartId)
            {
                try
                {
                    var response = await _httpClient.GetFromJsonAsync<List<CarritoItemDto>>($"https://localhost:7001/carritos/{cartId}");
                    ItemCount = response?.Sum(item => item.Cantidad) ?? 0;
                }
                catch (Exception)
                {
                    ItemCount = 0; 
                }
            }
        }
    }
}
