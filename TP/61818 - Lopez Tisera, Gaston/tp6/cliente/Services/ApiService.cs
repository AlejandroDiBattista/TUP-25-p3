using System.Net.Http.Json;
using Cliente.Modelos;

namespace Cliente.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> ObtenerProductosAsync(string? query = null)
    {
        var endpoint = string.IsNullOrWhiteSpace(query) ? "/productos" : $"/productos?query={query}";
        return await _http.GetFromJsonAsync<List<Producto>>(endpoint) ?? new List<Producto>();
    }
}
