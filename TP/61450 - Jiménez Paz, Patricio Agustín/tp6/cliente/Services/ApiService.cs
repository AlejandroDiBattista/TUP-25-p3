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

    public string? GetApiBaseUrl()
    {
        return _httpClient.BaseAddress?.ToString();
    }

    public async Task<List<Producto>> ObtenerDatosAsync(string? search = null)
    {
        try
        {
            string url = "/productos";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"?search={Uri.EscapeDataString(search)}";
            var response = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
            return response ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new List<Producto>
            {
                new Producto { Nombre = $"Error: {ex.Message}", Precio = 0 },
            };
        }
    }
}
