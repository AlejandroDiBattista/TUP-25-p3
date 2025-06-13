using System.Net.Http.Json;
using System.Text.Json;
using cliente.Models;

namespace cliente.Services
{
  public class ApiService
  {
    private readonly HttpClient _httpClient;
    private Guid? _carritoId;

    public event Action? OnCarritoActualizado;

    public ApiService(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    #region Productos

    public async Task<List<Producto>> ObtenerProductosAsync(string? query = null)
    {
      try
      {
        var url = "/productos";
        if (!string.IsNullOrWhiteSpace(query))
        {
          url += $"?query={Uri.EscapeDataString(query)}";
        }

        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
        return productos ?? new List<Producto>();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error al obtener productos: {ex.Message}");
        return new List<Producto>();
      }
    }

    #endregion
  }
}


