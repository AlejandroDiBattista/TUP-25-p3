using System.Net.Http.Json;
using TiendaOnline.Client.Models;

public class CompraService
{
    private readonly HttpClient _http;

    public CompraService(HttpClient http)
    {
        _http = http;
    }

    public async Task RealizarCompra(Compra compra)
    {
        await _http.PostAsJsonAsync("compras", compra);
    }
}

public async Task<List<Compra>> ObtenerCompras()
{
    return await _http.GetFromJsonAsync<List<Compra>>("compras") ?? new List<Compra>();
}
