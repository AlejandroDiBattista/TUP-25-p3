using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Servicios
{
    public class ServicioProductos
    {
        private readonly HttpClient _http;

        public ServicioProductos(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _http.GetFromJsonAsync<List<Producto>>("productos");
        }
    }
}