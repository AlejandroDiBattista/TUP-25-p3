using System.Net.Http.Json;
using Cliente.Models;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> ObtenerTodos()
    {
        var productos = await _http.GetFromJsonAsync<List<Producto>>("https://localhost:5001/productos");
        return productos ?? new List<Producto>();
    }

    
}