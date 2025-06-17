using System.Net.Http.Json;
using cliente.Models; 
using System.Linq; 

namespace cliente.Services 
{
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
                var response = await _httpClient.GetFromJsonAsync<List<CarritoItemDto>>($"/carritos/{cartId}");
                ItemCount = response?.Sum(item => item.Cantidad) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el contador del carrito en CartStateService: {ex.Message}");
                ItemCount = 0; 
            }
        }
    }
}
