using System.Net.Http.Json;
using cliente.Modelo;
using cliente.Services;

namespace cliente.Services
{
    public class CartService
    {
        private readonly HttpClient _http;
        public string? CarritoId { get; private set; }

        public CartService(HttpClient http)
        {
            _http = http;
        }

        public async Task InicializarCarrito()
        {
            if (CarritoId == null)
            {
                var resp = await _http.PostAsJsonAsync("/carritos", new { });
                if (resp.IsSuccessStatusCode)
                {
                    var data = await resp.Content.ReadFromJsonAsync<CarritoIdResponse>();
                    CarritoId = data?.CarritoId;
                }
            }
        }

        public event Action? OnCarritoActualizado;

        public async Task AgregarProducto(int productoId, int cantidad)
        {
            await InicializarCarrito();
            await _http.PutAsync($"/carritos/{CarritoId}/{productoId}?cantidad={cantidad}", null);
            OnCarritoActualizado?.Invoke();
        }

        public async Task<List<ItemCarritoDto>> ObtenerItems()
        {
            await InicializarCarrito();
            return await _http.GetFromJsonAsync<List<ItemCarritoDto>>($"/carritos/{CarritoId}") ?? new();
        }

        public async Task EliminarProducto(int productoId)
        {
            await InicializarCarrito();
            await _http.DeleteAsync($"/carritos/{CarritoId}/{productoId}");
            OnCarritoActualizado?.Invoke();
        }
        public async Task<int> ObtenerCantidadItems()
        {
            var items = await ObtenerItems();
            return items.Sum(i => i.Cantidad);
        }

        public async Task VaciarCarrito()
        {
            await InicializarCarrito();
            await _http.DeleteAsync($"/carritos/{CarritoId}");
            OnCarritoActualizado?.Invoke();
        }

        public async Task<bool> ConfirmarCompra(object compra)
        {
            await InicializarCarrito();
            var resp = await _http.PutAsJsonAsync($"/carritos/{CarritoId}/confirmar", compra);
            return resp.IsSuccessStatusCode;
        }

        private class CarritoIdResponse
        {
            public string CarritoId { get; set; }
        }
        public async Task ActualizarCantidad(int productoId, int cantidad)
        {
            await InicializarCarrito();
            await _http.PutAsync($"/carritos/{CarritoId}/{productoId}?cantidad={cantidad}", null);
            OnCarritoActualizado?.Invoke();
        }
    }
    

    public class ItemCarritoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
        public int Stock { get; set; }
    }
}

