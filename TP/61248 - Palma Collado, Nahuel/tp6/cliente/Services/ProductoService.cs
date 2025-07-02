using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Services
{
    public class ProductoService
    {
        private readonly HttpClient _http;

        public ProductoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
        {
            var url = "/api/productos";
            if (!string.IsNullOrWhiteSpace(busqueda))
                url += $"?busqueda={Uri.EscapeDataString(busqueda)}";

            var productos = await _http.GetFromJsonAsync<List<Producto>>(url);
            return productos ?? new List<Producto>();
        }
    }
}
