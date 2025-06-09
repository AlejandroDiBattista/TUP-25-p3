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

    // Obtener todos los productos (con búsqueda opcional)
    public async Task<List<Producto>> GetProductosAsync(string? q = null)
    {
        string url = "/api/productos";
        if (!string.IsNullOrWhiteSpace(q))
            url += $"?q={Uri.EscapeDataString(q)}";
        return await _httpClient.GetFromJsonAsync<List<Producto>>(url) ?? new List<Producto>();
    }

    // Crear un nuevo carrito y devolver el Guid
    public async Task<Guid> CrearCarritoAsync()
    {
        return await _httpClient.GetFromJsonAsync<Guid>("/api/carrito/crear");
    }

    // Obtener el carrito por ID
    public async Task<List<ItemCarrito>> ObtenerCarritoAsync(Guid carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCarrito>>($"/api/carrito/{carritoId}") ?? new List<ItemCarrito>();
    }

    // Agregar producto al carrito
    public async Task AgregarAlCarritoAsync(Guid carritoId, int productoId)
    {
        await _httpClient.PostAsync($"/api/carrito/{carritoId}/agregar/{productoId}", null);
    }

    // Quitar producto del carrito
    public async Task QuitarDelCarritoAsync(Guid carritoId, int productoId)
    {
        await _httpClient.PostAsync($"/api/carrito/{carritoId}/quitar/{productoId}", null);
    }

    // Vaciar carrito
    public async Task VaciarCarritoAsync(Guid carritoId)
    {
        await _httpClient.PostAsync($"/api/carrito/{carritoId}/vaciar", null);
    }

    // Confirmar compra
    public async Task<Compra> ConfirmarCompraAsync(Guid carritoId, Cliente cliente)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/carrito/{carritoId}/confirmar", cliente);
        return await response.Content.ReadFromJsonAsync<Compra>() ?? new Compra();
    }

    // Obtener datos generales (para Home.razor)
    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
        return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
    }
}