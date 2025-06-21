using System.Net.Http.Json;
using cliente.Modelo;
using cliente.Services;


namespace cliente.Services
{
    public class ProductoService
    {
        private readonly HttpClient _http;

        public ProductoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Producto>> ObtenerProductos(string? busqueda = null)
        {
            string url = "/productos";
            if (!string.IsNullOrWhiteSpace(busqueda))
                url += $"?q={busqueda}";
            return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new();
        }
    }
}