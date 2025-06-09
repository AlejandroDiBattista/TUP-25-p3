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

    // Crear un nuevo carrito
    public async Task<Guid> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/api/carritos", null);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    // Obtener el carrito por ID
    public async Task<List<ItemCarrito>> ObtenerCarritoAsync(Guid carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<ItemCarrito>>($"/api/carritos/{carritoId}") ?? new List<ItemCarrito>();
    }

    // Agregar producto al carrito
    public async Task AgregarAlCarritoAsync(Guid carritoId, int productoId)
    {
        await _httpClient.PutAsync($"/api/carritos/{carritoId}/{productoId}", null);
    }

    // Quitar producto del carrito
    public async Task QuitarDelCarritoAsync(Guid carritoId, int productoId)
    {
        await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
    }

    // Vaciar carrito
    public async Task VaciarCarritoAsync(Guid carritoId)
    {
        await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
    }

    // Confirmar compra
    public async Task<Compra> ConfirmarCompraAsync(Guid carritoId, Cliente cliente)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/compras?carritoId={carritoId}", cliente);
        return await response.Content.ReadFromJsonAsync<Compra>() ?? new Compra();
    }
}