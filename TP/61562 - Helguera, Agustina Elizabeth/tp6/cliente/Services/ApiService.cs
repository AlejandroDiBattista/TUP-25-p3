using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "Sin datos", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>("/productos");
            return productos ?? new List<Producto>();
        }
        catch
        {
            return new List<Producto>();
        }
    }
}
