using System.Net.Http.Json;

namespace Cliente.Services;

/// <summary>
/// Servicio para consumir la API de productos del backend.
/// </summary>
public class ProductoService
{
    private readonly HttpClient _http;
    private const string ApiUrl = "http://localhost:5184/productos";

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Obtiene la lista de productos desde la API. Permite búsqueda opcional por query.
    /// </summary>
    public async Task<List<ProductoDto>> GetProductosAsync(string? query = null)
    {
        string url = ApiUrl;
        if (!string.IsNullOrWhiteSpace(query))
            url += $"?query={Uri.EscapeDataString(query)}";
        var productos = await _http.GetFromJsonAsync<List<ProductoDto>>(url);
        return productos ?? new List<ProductoDto>();
    }
}

/// <summary>
/// Modelo de datos para representar un producto en el cliente.
/// </summary>
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}
