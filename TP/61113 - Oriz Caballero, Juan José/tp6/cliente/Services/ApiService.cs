using System.Net.Http.Json;
using Cliente.Modelo;

namespace Cliente.Services
{
    #nullable enable
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Producto>> ObtenerProductosAsync(string? filtro = null)
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(filtro)
                    ? "/productos"
                    : $"/productos?search={Uri.EscapeDataString(filtro)}";

                Console.WriteLine($"[ApiService] GET {url}");

                var productos = await _http.GetFromJsonAsync<List<Producto>>(url);
                Console.WriteLine($"[ApiService] Productos obtenidos: {productos?.Count ?? 0}");

                return productos ?? new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApiService] ERROR al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }
    }
}