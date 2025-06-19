using System.Net.Http.Json;

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
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }
    public async Task<List<Producto>> ObtenerProductosAsync(string? consulta = null)
    {
        try
        {
            string url = "/productos";
            if (!string.IsNullOrWhiteSpace(consulta))
                url += $"?q={Uri.EscapeDataString(consulta)}";

            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
    public async Task<string> ConfirmarCompraAsync(int carritoId, object cliente)
    {
        var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", cliente);
        if (response.IsSuccessStatusCode)
            return "ok";
        var errorMsg = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(errorMsg) ? "Ocurrió un error al confirmar la compra." : errorMsg;
    }
    public async Task<int> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/carritos", null);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            return json?["carritoId"] ?? 0;
        }
        return 0;
    }
    public async Task<bool> AgregarOActualizarProductoEnCarritoAsync(int carritoId, int productoId, int cantidad)
    {
        var body = new { cantidad };
        var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/{productoId}", body);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> QuitarProductoDelCarritoAsync(int carritoId, int productoId)
    {
        var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> VaciarCarritoAsync(int carritoId)
    {
        var response = await _httpClient.DeleteAsync($"carritos/{carritoId}");
        return response.IsSuccessStatusCode;
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
