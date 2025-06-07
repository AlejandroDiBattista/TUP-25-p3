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

        public async Task<List<Producto>> ObtenerProductos()
        {
            return await _http.GetFromJsonAsync<List<Producto>>("/productos") ?? new List<Producto>();
        }
    }
}

