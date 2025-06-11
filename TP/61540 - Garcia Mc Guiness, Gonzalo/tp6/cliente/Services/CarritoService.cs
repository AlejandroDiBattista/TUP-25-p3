using System.Net.Http.Json;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        public int? CarritoId { get; private set; }

        public CarritoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> GetOClearCarritoAsync()
        {
            if (CarritoId.HasValue) return CarritoId.Value;
            var resp = await _http.PostAsync("/carritos", null);
            var data = await resp.Content.ReadFromJsonAsync<CarritoIdResponse>();
            CarritoId = data?.CarritoId ?? 0;
            return CarritoId.Value;
        }

        public async Task AgregarProductoAsync(int productoId, int cantidad)
        {
            var carritoId = await GetOClearCarritoAsync();
            await _http.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        }

        public async Task<Carrito?> GetCarritoAsync()
        {
            if (!CarritoId.HasValue) return null;
            return await _http.GetFromJsonAsync<Carrito>($"/carritos/{CarritoId}");
        }
        public async Task QuitarProductoAsync(int productoId)
        {
            if (!CarritoId.HasValue) return;
            await _http.DeleteAsync($"/carritos/{CarritoId}/{productoId}");
        }

        public async Task VaciarCarritoAsync()
        {
            if (!CarritoId.HasValue) return;
            await _http.DeleteAsync($"/carritos/{CarritoId}");
        }

    }

    public class CarritoIdResponse
    {
        public int CarritoId { get; set; }
    }

    public class Carrito
    {
        public int Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
    }

    public class ItemCarrito
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = new();
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}