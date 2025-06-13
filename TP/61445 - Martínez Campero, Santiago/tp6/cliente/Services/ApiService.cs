#nullable enable
using System.Net.Http.Json;
using Compartido;
using System.Text.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<Producto[]?> GetProductosAsync(string? busqueda = null)
    {
        var url = "/api/productos";
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            url += $"?busqueda={Uri.EscapeDataString(busqueda)}";
        }
        return await _httpClient.GetFromJsonAsync<Producto[]>(url, _jsonSerializerOptions);
    }

    public async Task<Producto?> GetProductoAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Producto>($"/api/productos/{id}", _jsonSerializerOptions);
    }

    public async Task<Compra?> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/api/carritos", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Compra>(_jsonSerializerOptions);
    }

    public async Task<List<ItemCarritoDto>?> GetCarritoAsync(Guid carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCarritoDto>>($"/api/carritos/{carritoId}", _jsonSerializerOptions);
    }

    public async Task<ItemCarritoActualizadoDto?> AgregarOActualizarItemCarritoAsync(Guid carritoId, int productoId, int cantidad)
    {
        var request = new CantidadRequest { Cantidad = cantidad };
        var response = await _httpClient.PutAsJsonAsync($"/api/carritos/{carritoId}/{productoId}", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ItemCarritoActualizadoDto>(_jsonSerializerOptions);
        }
        else
        {
            // Handle error (e.g., log, throw exception, return null or a specific error DTO)
            // For now, returning null or re-throwing might be simplest
            // var errorContent = await response.Content.ReadAsStringAsync();
            // Console.WriteLine($"Error: {response.StatusCode}, {errorContent}");
            response.EnsureSuccessStatusCode(); // This will throw for non-success codes
            return null; // Should not be reached if EnsureSuccessStatusCode throws
        }
    }

    public async Task<bool> EliminarItemCarritoAsync(Guid carritoId, int productoId)
    {
        var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VaciarCarritoAsync(Guid carritoId)
    {
        var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<ConfirmacionCompraResponseDto?> ConfirmarCompraAsync(Guid carritoId, ConfirmarCompraRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ConfirmacionCompraResponseDto>(_jsonSerializerOptions);
    }

    // Removed old ObtenerDatosAsync and DatosRespuesta as they are not part of the core requirements
}
