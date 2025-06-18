#nullable enable
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Cliente.Models;

namespace Cliente.Services;

public class ApiService 
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger) 
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Producto>> GetProductosAsync(string? query = null)
    {
        try
        {
            var url = "/productos";
            if (!string.IsNullOrWhiteSpace(query))
            {
                url += $"?q={Uri.EscapeDataString(query)}";
                _logger.LogInformation("Buscando productos con query: '{Query}'", query);
            }
            else
            {
                _logger.LogDebug("Obteniendo todos los productos");
            }
            
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url) ?? new();
            _logger.LogInformation("Obtenidos {Count} productos", productos.Count);
            return productos;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al obtener productos");
            throw new InvalidOperationException("Error de conexión con el servidor. Verifique su conexión a internet.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener productos");
            throw;
        }
    }

    public HttpClient HttpClient => _httpClient;
}
