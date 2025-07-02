using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        private int? _carritoId;
        private List<ItemCompra> _items = new();

        public event Action? OnChange;

        public CarritoService(HttpClient http)
        {
            _http = http;
        }

        // Propiedades expuestas para el componente
        public IReadOnlyList<ItemCompra> Items => _items;
        public int TotalItems => _items.Sum(i => i.Cantidad);
        public decimal TotalPrecio => _items.Sum(i => i.Cantidad * i.PrecioUnitario);

        // Inicializa o devuelve el carrito actual
        private async Task<int> GetOrCreateCartAsync()
        {
            if (_carritoId.HasValue)
                return _carritoId.Value;

            var resp = await _http.PostAsJsonAsync("carritos", new { });
            resp.EnsureSuccessStatusCode();
            _carritoId = await resp.Content.ReadFromJsonAsync<int>();
            return _carritoId.Value;
        }

        // Carga items desde el servidor
        public async Task<List<ItemCompra>> LoadItemsAsync()
        {
            var id = await GetOrCreateCartAsync();
            _items = await _http
                .GetFromJsonAsync<List<ItemCompra>>($"carritos/{id}")
                ?? new List<ItemCompra>();
            NotifyStateChanged();
            return _items;
        }

        // Agregar o actualizar cantidad
        public async Task AddOrUpdateItemAsync(int productoId, int cantidad)
        {
            var id = await GetOrCreateCartAsync();
            var resp = await _http.PutAsync(
                $"carritos/{id}/{productoId}?cantidad={cantidad}", null);
            resp.EnsureSuccessStatusCode();
            await LoadItemsAsync();
        }

        // Eliminar o reducir cantidad
        public async Task RemoveItemAsync(int productoId)
        {
            var id = await GetOrCreateCartAsync();
            var resp = await _http.DeleteAsync($"carritos/{id}/{productoId}");
            resp.EnsureSuccessStatusCode();
            await LoadItemsAsync();
        }

        // Vaciar carrito
        public async Task ClearCartAsync()
        {
            var id = await GetOrCreateCartAsync();
            var resp = await _http.DeleteAsync($"carritos/{id}");
            resp.EnsureSuccessStatusCode();
            await LoadItemsAsync();
        }

        // Confirmar compra (usa el DTO correspondiente)
        public async Task ConfirmPurchaseAsync(CheckoutDto dto)
        {
            var id = await GetOrCreateCartAsync();
            var resp = await _http.PutAsJsonAsync(
                $"carritos/{id}/confirmar",
                dto
            );
            resp.EnsureSuccessStatusCode();

            // Limpiar estado tras la confirmación
            _carritoId = null;
            _items.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
