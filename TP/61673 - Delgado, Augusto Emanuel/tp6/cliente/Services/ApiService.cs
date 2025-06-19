#nullable enable 

using System.Net.Http.Json;
using modelos_compartidos; 

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

    
    public async Task<List<Producto>?> GetProductos()
    {
        try
        {
           
            return await _httpClient.GetFromJsonAsync<List<Producto>>("productos");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al cargar productos desde la API: {ex.Message}");
            return null;
        }
    }

    
    public async Task<bool> UpdateStock(List<StockUpdateDto> stockUpdates)
    {
        try
        {
           
            var response = await _httpClient.PostAsJsonAsync("stock/update", stockUpdates);
            response.EnsureSuccessStatusCode();
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

public class DatosRespuesta
{
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
