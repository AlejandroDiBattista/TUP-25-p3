using Microsoft.JSInterop;
using System.Net.Http.Json;
using cliente.models;
#nullable enable
namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        public event Action? CarritoActualizado;
        private int _contadorCarrito;
        public int ContadorCarrito
        {
            get => _contadorCarrito;
            private set
            {
                if (_contadorCarrito != value)
                {
                    _contadorCarrito = value;
                    CarritoActualizado?.Invoke();
                }
            }
        }
        public void EstablecerContadorCarrito(int nuevoValor)
        {
            ContadorCarrito = nuevoValor;
        }
        public ApiService(HttpClient http) => _http = http;

        public async Task<List<Producto>> GetProductos(string? busqueda = null)
        {
            var url = "/productos" + (string.IsNullOrWhiteSpace(busqueda) ? "" : $"?busqueda={busqueda}");
            return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new();
        }

        public async Task<int> CrearCarrito()
        {
            var resp = await _http.PostAsync("/carritos", null);
            return int.Parse(await resp.Content.ReadAsStringAsync());
        }

        public async Task<List<Compras>> GetCarrito(int carritoId)
        {
            return await _http.GetFromJsonAsync<List<Compras>>($"/carritos/{carritoId}") ?? new();
        }

        public async Task VaciarCarrito(int carritoId)
        {
            await _http.DeleteAsync($"/carritos/{carritoId}");
        }

        public async Task ConfirmarCompra(int carritoId, ConfirmarCompra confirmacion)
        {
            var response = await _http.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", confirmacion);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al confirmar compra: {response.StatusCode} - {error}");
            }
        }

        public async Task AgregarOActualizarProducto(int carritoId, int productoId, int cantidad)
        {
            var response = await _http.PutAsJsonAsync($"/carritos/{carritoId}/{productoId}", cantidad);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ ERROR: {response.StatusCode} - {error}");
                throw new Exception($"Error al agregar/actualizar producto: {response.StatusCode} - {error}");
            }
        }

        public async Task EliminarProducto(int carritoId, int productoId)
        {
            await _http.DeleteAsync($"/carritos/{carritoId}/{productoId}");
        }

        public async Task ActualizarContadorCarrito(int carritoId)
        {
            var items = await GetCarrito(carritoId);
            ContadorCarrito = items.Sum(i => i.Cantidad);
        }

        public class CarritoStorage
        {
            private readonly IJSRuntime js;
            public CarritoStorage(IJSRuntime js) => this.js = js;

            public async Task GuardarCarritoId(int id) =>
                await js.InvokeVoidAsync("localStorage.setItem", "carritoId", id.ToString());

            public async Task<int?> ObtenerCarritoId()
            {
                var idStr = await js.InvokeAsync<string>("localStorage.getItem", "carritoId");
                return int.TryParse(idStr, out var id) ? id : (int?)null;
            }

            public async Task LimpiarCarritoId() =>
                await js.InvokeVoidAsync("localStorage.removeItem", "carritoId");
        }  
    }
}