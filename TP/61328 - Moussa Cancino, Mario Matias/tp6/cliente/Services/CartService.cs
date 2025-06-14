using cliente.Modelos;
using System.Linq;

namespace cliente.Services
{
    public class CartService
    {
        public event Action OnChange;
        private readonly ApiService _apiService;
        private Guid? _cartId;
        private Dictionary<int, int> _items = new Dictionary<int, int>();
        public IReadOnlyDictionary<int, int> Items => _items;
        private Dictionary<int, Producto> _productDetails = new Dictionary<int, Producto>();

        public CartService(ApiService apiService) { _apiService = apiService; }

        public async Task AddToCartAsync(Producto producto) {
            await EnsureCartExistsAsync();
            await _apiService.AddProductToCartAsync(_cartId.Value, producto.Id);
            if (_items.ContainsKey(producto.Id)) {
                _items[producto.Id]++;
            } else {
                _items[producto.Id] = 1;
                _productDetails[producto.Id] = producto;
            }
            NotifyStateChanged();
        }

        public async Task RemoveFromCartAsync(int productId) {
            if (!_cartId.HasValue || !_items.ContainsKey(productId)) return;
            await _apiService.RemoveProductFromCartAsync(_cartId.Value, productId);
            _items[productId]--;
            if (_items[productId] == 0) {
                _items.Remove(productId);
            }
            NotifyStateChanged();
        }

        public async Task EmptyCartAsync() {
            if (!_cartId.HasValue) return;
            await _apiService.EmptyCartAsync(_cartId.Value);
            _items.Clear();
            NotifyStateChanged();
        }
        
        public async Task ConfirmPurchaseAsync(DatosCliente datosCliente) {
            if (!_cartId.HasValue || !_items.Any()) return;
            await _apiService.ConfirmPurchaseAsync(_cartId.Value, datosCliente);
            _items.Clear();
            _cartId = null; 
            NotifyStateChanged();
        }

        public int GetItemCount() => _items.Sum(i => i.Value);
        public Producto GetProductDetails(int productId) => _productDetails.GetValueOrDefault(productId);
        private async Task EnsureCartExistsAsync() { if (!_cartId.HasValue) { _cartId = await _apiService.CreateCartAsync(); } }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}