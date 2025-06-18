using System.Net.Http.Json;
using Cliente.Models;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<List<Producto>> ObtenerDatosAsync() {
    try {
        var response = await _httpClient.GetFromJsonAsync<List<Producto>>("productos");
        return response ?? new List<Producto>();
    } catch (Exception ex) {
        Console.WriteLine($"Error al obtener datos: {ex.Message}");
        return new List<Producto>();
    }
}
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
