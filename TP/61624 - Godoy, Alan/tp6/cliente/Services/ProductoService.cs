using System.Net.Http.Json;
using TiendaOnline.Client.Models;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> ObtenerProductos()
    {
        return await _http.GetFromJsonAsync<List<Producto>>("productos");
    }
}
