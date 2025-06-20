using System.Net.Http.Json;
using cliente.Models;

public class ProductoService
{
    private readonly HttpClient _http;
    
    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> GetProductosAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search) ? "/productos" : $"/productos?search={search}";
        return await _http.GetFromJsonAsync<List<Producto>>(url);
    }

    public async Task ActualizarStock(int productoId, int cantidadDescontada)
    {
        var productos = await GetProductosAsync();
        var producto = productos.FirstOrDefault(p => p.Id == productoId);
        if (producto != null)
        {
            producto.Stock -= cantidadDescontada;
        }
    }
}