using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

/// <summary>
/// Servicio para gestionar operaciones relacionadas con productos
/// </summary>
public class ProductosService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de productos
    /// </summary>
    /// <param name="httpClient">Cliente HTTP para realizar peticiones al servidor</param>
    public ProductosService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Obtiene la lista de productos con la opción de filtrar por búsqueda
    /// </summary>
    /// <param name="busqueda">Texto opcional para filtrar productos</param>
    /// <returns>Lista de productos que coinciden con el criterio de búsqueda</returns>
    public async Task<List<ProductoDTO>> ObtenerProductosAsync(string? busqueda = null)
    {
        try
        {
            var url = "/api/productos";
            if (!string.IsNullOrEmpty(busqueda))
            {
                url += $"?busqueda={Uri.EscapeDataString(busqueda)}";
            }

            var productos = await _httpClient.GetFromJsonAsync<List<ProductoDTO>>(url);
            return productos ?? new List<ProductoDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<ProductoDTO>();
        }
    }
}
