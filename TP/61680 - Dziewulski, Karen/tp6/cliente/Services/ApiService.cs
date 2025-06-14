using System.Net.Http.Json;
using cliente.Modelos;

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
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<List<Producto>> ObtenerProductosAsync(string? q = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(q) ? "/productos" : $"/productos?q={Uri.EscapeDataString(q)}";
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
