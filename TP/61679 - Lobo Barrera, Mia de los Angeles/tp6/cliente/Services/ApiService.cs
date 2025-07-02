using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Producto>> ObtenerProductos(string? query = null)
    {
        string url = "productos";
        if (!string.IsNullOrWhiteSpace(query))
            url += $"?query={Uri.EscapeDataString(query)}";

        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
        return productos ?? new List<Producto>();
    }
}