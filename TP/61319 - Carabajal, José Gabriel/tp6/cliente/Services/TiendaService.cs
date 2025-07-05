using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services
{
    public class TiendaService
    {
        private readonly HttpClient _http;
        private int? _carritoId;

        public TiendaService(HttpClient http)
        {
            _http = http;
        }

        // 1) Obtener productos (opcional búsqueda)
        public async Task<List<Producto>> GetProductosAsync(string? busqueda = null)
        {
            var url = string.IsNullOrWhiteSpace(busqueda)
                ? "productos"
                : $"productos?q={Uri.EscapeDataString(busqueda)}";
            return await _http.GetFromJsonAsync<List<Producto>>(url)
                ?? new List<Producto>();
        }

        // 2) Crear o devolver carrito existente
        public async Task<int> CrearCarritoAsync()
        {
            if (_carritoId.HasValue)
                return _carritoId.Value;

            var response = await _http.PostAsJsonAsync("carritos", new { });
            response.EnsureSuccessStatusCode();
            _carritoId = await response.Content.ReadFromJsonAsync<int>();
            return _carritoId.Value;
        }

        // 3) Obtener items del carrito
        public async Task<List<ItemCompra>> GetCarritoAsync()
        {
            var id = await CrearCarritoAsync();
            return await _http.GetFromJsonAsync<List<ItemCompra>>($"carritos/{id}")
                ?? new List<ItemCompra>();
        }

        // 4) Vaciar carrito
        public async Task VaciarCarritoAsync()
        {
            var id = await CrearCarritoAsync();
            var resp = await _http.DeleteAsync($"carritos/{id}");
            resp.EnsureSuccessStatusCode();
        }

        // 5) Confirmar compra (envía datos de cliente)
        public async Task ConfirmarCompraAsync(string nombre, string apellido, string email)
        {
            var id = await CrearCarritoAsync();
            var datos = new Compra {
                NombreCliente = nombre,
                ApellidoCliente = apellido,
                EmailCliente = email
            };
            var resp = await _http.PutAsJsonAsync($"carritos/{id}/confirmar", datos);
            resp.EnsureSuccessStatusCode();
            _carritoId = null; // empezamos de nuevo
        }

        // 6) Agregar o actualizar cantidad
        public async Task<ItemCompra> AgregarItemAsync(int productoId, int cantidad)
        {
            var id = await CrearCarritoAsync();
            var resp = await _http.PutAsync(
                $"carritos/{id}/{productoId}?cantidad={cantidad}",
                null);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ItemCompra>()
                ?? throw new Exception("No vino item en la respuesta");
        }

        // 7) Eliminar item o reducir cantidad
        public async Task EliminarItemAsync(int productoId)
        {
            var id = await CrearCarritoAsync();
            var resp = await _http.DeleteAsync($"carritos/{id}/{productoId}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
