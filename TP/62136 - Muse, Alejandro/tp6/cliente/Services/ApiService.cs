using System.Net.Http.Json;
using cliente.Models; // Asegurate de tener el modelo Producto en cliente

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    // Método para obtener los productos del backend
    public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
    {
        string url = "/productos";
        if (!string.IsNullOrWhiteSpace(busqueda))
            url += $"?q={Uri.EscapeDataString(busqueda)}";

        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
        return productos ?? new List<Producto>();
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
