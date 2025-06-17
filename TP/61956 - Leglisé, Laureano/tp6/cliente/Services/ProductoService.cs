using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> ObtenerProductos(string? buscar = null)
    {
        string url = "/productos";
        if (!string.IsNullOrWhiteSpace(buscar))
            url += $"?buscar={buscar}";

        return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new();
    }
}
