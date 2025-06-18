using System.Net.Http.Json;
using cliente.Models;

public class ProductoService
{
    private readonly HttpClient _http;
    
    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> GetProductosAsync(string search = "")
    {
        try
        {
            var productos = await _http.GetFromJsonAsync<List<Producto>>($"/productos?search={search}");
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
}