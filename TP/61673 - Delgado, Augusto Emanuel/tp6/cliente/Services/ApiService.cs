#nullable enable // Agregado para habilitar el contexto de anotaciones de tipos de referencia que aceptan valores NULL

using System.Net.Http.Json;
using modelos_compartidos; // Asegúrate de que este namespace contenga Producto y StockUpdateDto

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Método para obtener datos genéricos (tu código existente)
    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        try
        {
            // Este endpoint '/api/datos' es el que tenías en tu código original
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    // Método para obtener la lista de productos desde la API (implementación anterior)
    public async Task<List<Producto>?> GetProductos()
    {
        try
        {
            // Este endpoint debe coincidir con el mapeo en tu servidor/Program.cs (app.MapGet("/productos", ...))
            return await _httpClient.GetFromJsonAsync<List<Producto>>("productos");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al cargar productos desde la API: {ex.Message}");
            return null; // Retorna null o una lista vacía en caso de error
        }
    }

    // Método para enviar las actualizaciones de stock al backend (nueva implementación)
    public async Task<bool> UpdateStock(List<StockUpdateDto> stockUpdates)
    {
        try
        {
            // Este endpoint debe coincidir con el mapeo en tu servidor/Program.cs (app.MapPost("/stock/update", ...))
            var response = await _httpClient.PostAsJsonAsync("stock/update", stockUpdates);
            response.EnsureSuccessStatusCode(); // Lanza una excepción si el código de estado HTTP indica un error.
            Console.WriteLine("Stock actualizado exitosamente en el servidor.");
            return true;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al actualizar stock en la API: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado al actualizar stock: {ex.Message}");
            return false;
        }
    }
}

// Tu clase DatosRespuesta existente
public class DatosRespuesta
{
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
