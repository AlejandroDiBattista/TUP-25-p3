using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;

        public CarritoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<Carrito> CrearCarritoAsync()
        {
            var response = await _http.PostAsync("api/carritos", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Carrito>();
        }

        public async Task<Carrito> ObtenerCarritoAsync(string carritoId)
        {
            return await _http.GetFromJsonAsync<Carrito>($"api/carritos/{carritoId}");
        }

        public async Task AgregarProductoAsync(string carritoId, int productoId, int cantidad)
        {
            var response = await _http.PutAsJsonAsync($"api/carritos/{carritoId}/{productoId}?cantidad={cantidad}", new { });
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarProductoAsync(string carritoId, int productoId)
        {
            var response = await _http.DeleteAsync($"api/carritos/{carritoId}/{productoId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task VaciarCarritoAsync(string carritoId)
        {
            var response = await _http.DeleteAsync($"api/carritos/{carritoId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ConfirmarCompraAsync(string carritoId, ClienteDto cliente)
        {
            var response = await _http.PutAsJsonAsync($"api/carritos/{carritoId}/confirmar", cliente);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(error);
            }
        }
    }
}