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

    public async Task<List<Producto>> ObtenerProductos()
    {
        var productos = await _http.GetFromJsonAsync<List<Producto>>("productos");
        return productos ?? new List<Producto>();
    }


}
