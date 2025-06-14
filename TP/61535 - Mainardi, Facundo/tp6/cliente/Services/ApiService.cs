using System.Net.Http.Json;
using Cliente.Modelos;
namespace Cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var respuesta = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return respuesta ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }
    public async Task<List<Producto>> BuscarProductos(string filtro)
    {
        try
        {
            // Si el filtro está vacío, llama al endpoint sin parámetros
            string url = string.IsNullOrWhiteSpace(filtro)
                ? "/productos"
                : $"/productos?q={Uri.EscapeDataString(filtro)}";

            var response = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
            return response ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}