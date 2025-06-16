using System.Net.Http.Json;
using cliente.Modulos;
using Microsoft.JSInterop;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private string? carritoId;

        public event Action? OnChange;

        private int _contadorCarrito = 0;
        public int ContadorCarrito
        {
            get => _contadorCarrito;
            private set
            {
                if (_contadorCarrito != value)
                {
                    _contadorCarrito = value;
                    NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public ApiService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task InicializarCarritoIdAsync()
        {
            carritoId = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");
            if (string.IsNullOrWhiteSpace(carritoId))
            {
                carritoId = Guid.NewGuid().ToString();
                await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", carritoId);
            }
        }

        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
        {
            string url = "/api/productos";
            if (!string.IsNullOrWhiteSpace(busqueda))
                url += $"?busqueda={Uri.EscapeDataString(busqueda)}";
            return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new List<Producto>();
        }

        public async Task<List<ItemCompra>> ObtenerCarritoAsync()
        {
            await InicializarCarritoIdAsync();
            var items = await _http.GetFromJsonAsync<List<ItemCompra>>($"/api/carritos/{carritoId}") ?? new List<ItemCompra>();
            ContadorCarrito = items.Sum(i => i.Cantidad);
            return items;
        }

        public async Task InicializarCarritoAsync()
        {
            await InicializarCarritoIdAsync();
            await _http.PostAsync($"/api/carritos/{carritoId}", null);
            ContadorCarrito = 0;
        }

        public async Task<List<ItemCompra>> AgregarAlCarritoAsync(int productoId, int cantidad)
        {
            await InicializarCarritoIdAsync();
            var resp = await _http.PutAsync($"/api/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
            var items = await resp.Content.ReadFromJsonAsync<List<ItemCompra>>() ?? new List<ItemCompra>();
            ContadorCarrito = items.Sum(i => i.Cantidad);
            return items;
        }

        public async Task<List<ItemCompra>> QuitarDelCarritoAsync(int productoId, int cantidad)
        {
            await InicializarCarritoIdAsync();
            var resp = await _http.DeleteAsync($"/api/carritos/{carritoId}/{productoId}?cantidad={cantidad}");
            var items = await resp.Content.ReadFromJsonAsync<List<ItemCompra>>() ?? new List<ItemCompra>();
            ContadorCarrito = items.Sum(i => i.Cantidad);
            return items;
        }

        public async Task VaciarCarritoAsync()
        {
            await InicializarCarritoIdAsync();
            await _http.DeleteAsync($"/api/carritos/{carritoId}");
            ContadorCarrito = 0;
        }

        public async Task<Compra> ConfirmarCompraAsync(RequisitosCompra datos)
        {
            await InicializarCarritoIdAsync();
            var resp = await _http.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", datos);
            resp.EnsureSuccessStatusCode();
            ContadorCarrito = 0;
            return await resp.Content.ReadFromJsonAsync<Compra>() ?? throw new Exception("Error al confirmar la compra");
        }
    }
}
