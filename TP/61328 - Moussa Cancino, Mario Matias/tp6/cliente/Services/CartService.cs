using cliente.Modelos;

namespace cliente.Services
{
    public class CartService
    {
        public event Action OnChange;
        private readonly ApiService _apiService;
        private Guid? _cartId;
        private List<Producto> _items = new List<Producto>();
        public IReadOnlyList<Producto> Items => _items.AsReadOnly();

        public CartService(ApiService apiService) { _apiService = apiService; }

        public async Task AddToCartAsync(Producto producto) {
            await EnsureCartExistsAsync();
            await _apiService.AddProductToCartAsync(_cartId.Value, producto.Id);
            _items.Add(producto);
            NotifyStateChanged();
        }

        public async Task EmptyCartAsync() {
            if (!_cartId.HasValue) return;
            await _apiService.EmptyCartAsync(_cartId.Value);
            _items.Clear();
            NotifyStateChanged();
        }
        public async Task ConfirmPurchaseAsync(DatosCliente datosCliente)
        {
            if (!_cartId.HasValue || !_items.Any()) return;

            await _apiService.ConfirmPurchaseAsync(_cartId.Value, datosCliente);
            _items.Clear();
            _cartId = null; 
            NotifyStateChanged();
        }

        public int GetItemCount() => _items.Count;
        private async Task EnsureCartExistsAsync() { if (!_cartId.HasValue) { _cartId = await _apiService.CreateCartAsync(); } }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}