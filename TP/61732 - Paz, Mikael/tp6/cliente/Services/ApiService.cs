using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    // Obtener productos
    public async Task<List<ProductoDto>?> ObtenerProductosAsync(string? q = null) {
        var url = "/productos" + (string.IsNullOrWhiteSpace(q) ? "" : $"?q={q}");
        return await _httpClient.GetFromJsonAsync<List<ProductoDto>>(url);
    }

    // Carrito
    public async Task<CarritoDto?> CrearCarritoAsync() =>
        await _httpClient.PostAsJsonAsync("/carritos", new { }).ContinueWith(t => t.Result.Content.ReadFromJsonAsync<CarritoDto>()).Unwrap();

    public async Task<CarritoDto?> ObtenerCarritoAsync(Guid carritoId) =>
        await _httpClient.GetFromJsonAsync<CarritoDto>($"/carritos/{carritoId}");

    public async Task<CarritoDto?> VaciarCarritoAsync(Guid carritoId) =>
        await _httpClient.DeleteAsync($"/carritos/{carritoId}").ContinueWith(t => t.Result.Content.ReadFromJsonAsync<CarritoDto>()).Unwrap();

    public async Task<CarritoDto?> AgregarProductoAsync(Guid carritoId, int productoId, int cantidad) =>
        await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/{productoId}", cantidad).ContinueWith(t => t.Result.Content.ReadFromJsonAsync<CarritoDto>()).Unwrap();

    public async Task<CarritoDto?> QuitarProductoAsync(Guid carritoId, int productoId) =>
        await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}").ContinueWith(t => t.Result.Content.ReadFromJsonAsync<CarritoDto>()).Unwrap();

    public async Task<CompraDto?> ConfirmarCompraAsync(Guid carritoId, CompraDto compra) =>
        await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", compra).ContinueWith(t => t.Result.Content.ReadFromJsonAsync<CompraDto>()).Unwrap();

    // ...método de ejemplo existente...
    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }
}
