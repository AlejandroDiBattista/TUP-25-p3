using System.Net.Http.Json;
using cliente.Dtos;

namespace cliente.Services;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductoDto>> ObtenerProductos(string? buscar = null)
    {
        try
        {
            var url = "productos" + (string.IsNullOrWhiteSpace(buscar) ? "" : $"?buscar={buscar}");
            return await _http.GetFromJsonAsync<List<ProductoDto>>(url) ?? new List<ProductoDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<ProductoDto>();
        }
    }
}