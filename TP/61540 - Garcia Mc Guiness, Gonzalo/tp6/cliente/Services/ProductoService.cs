using System.Net.Http.Json;

public class ProductoService
{
    private readonly HttpClient _http;
    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> GetProductosAsync(string? q = null)
    {
        string url = "/productos";
        if (!string.IsNullOrWhiteSpace(q))
            url += $"?q={q}";
        return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new List<Producto>();
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = "";
    public int StockTotal { get; set; }
}