using System.Net.Http.Json;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> ObtenerProductos(string? buscar = null)
    {
        var url = "/api/productos";
        if (!string.IsNullOrWhiteSpace(buscar))
            url += $"?buscar={buscar}";

        return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new();
    }
    public async Task<Producto?> ObtenerPorId(int id)
    {
        return await _http.GetFromJsonAsync<Producto>($"/api/productos/{id}");
    }
}