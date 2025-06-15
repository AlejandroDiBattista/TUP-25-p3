using System.Net.Http.Json;

namespace cliente.Services;
public class ProductoService
{
    private readonly HttpClient _httpClient;

    public ProductoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Producto>> ObtenerProductosAsync(string? query = null)
    {
        string url = "productos";
        if (!string.IsNullOrWhiteSpace(query))
            url += $"?q={query}";

        return await _httpClient.GetFromJsonAsync<List<Producto>>(url);
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}
