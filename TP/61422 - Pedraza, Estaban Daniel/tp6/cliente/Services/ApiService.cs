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

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/productos");
            response.EnsureSuccessStatusCode();
            var productos = await response.Content.ReadFromJsonAsync<List<Producto>>();

            return productos ?? new List<Producto>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error HTTP al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}");
            return new List<Producto>();
        }
    }
}
