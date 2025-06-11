#nullable enable
using System.Net.Http.Json;
using Cliente.Models;

namespace Cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<List<Producto>> GetProductosAsync(string? query = null)
    {
        var url = "/productos" + (string.IsNullOrWhiteSpace(query) ? "" : $"?q={query}");
        return await _httpClient.GetFromJsonAsync<List<Producto>>(url) ?? new();
    }

    public HttpClient HttpClient => _httpClient;
}
