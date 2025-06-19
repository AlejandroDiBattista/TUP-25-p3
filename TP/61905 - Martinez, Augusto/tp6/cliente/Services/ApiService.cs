using System.Net.Http.Json;
using Cliente.Models;

namespace Cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ✅ Método mejorado para obtener productos de la API
    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/productos");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error al obtener productos: {response.StatusCode}");
                return new List<Producto>();
            }

            var productos = await response.Content.ReadFromJsonAsync<List<Producto>>();
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error crítico al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
}
