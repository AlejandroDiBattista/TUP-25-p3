using System.Net.Http.Json;
using Cliente.Models;
namespace Cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

public async Task<List<Producto>> ObtenerProductosAsync() {
        try {
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>("/api/productos");
            return productos ?? new List<Producto>();
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
}    

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }

    }
}