using System.Net.Http.Json;
using shared.Models;
using shared.DTOs;

namespace cliente.Services;

public class TiendaService
{
    private readonly HttpClient _httpClient;
    private string? _carritoId;

    public TiendaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Producto>> GetProductosAsync(string? busqueda = null)
    {
        var url = "/productos";
        if (!string.IsNullOrEmpty(busqueda))
        {
            url += $"?q={Uri.EscapeDataString(busqueda)}";
        }
        
        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
        return productos ?? new List<Producto>();
    }

    public async Task<string> InicializarCarritoAsync()
    {
        if (string.IsNullOrEmpty(_carritoId))
        {
            var response = await _httpClient.PostAsJsonAsync("/carritos", new { });
            var carritoResponse = await response.Content.ReadFromJsonAsync<CarritoResponse>();
            _carritoId = carritoResponse?.CarritoId ?? string.Empty;
        }
        return _carritoId;
    }

    public async Task<List<ItemCarrito>> GetItemsCarritoAsync()
    {
        if (string.IsNullOrEmpty(_carritoId))
            return new List<ItemCarrito>();

        var items = await _httpClient.GetFromJsonAsync<List<ItemCarrito>>($"/carritos/{_carritoId}");
        return items ?? new List<ItemCarrito>();
    }

    public async Task<bool> AgregarAlCarritoAsync(int productoId, int cantidad = 1)
    {
        await InicializarCarritoAsync();
        
        var response = await _httpClient.PutAsJsonAsync($"/carritos/{_carritoId}/{productoId}", new { cantidad });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarDelCarritoAsync(int productoId, int cantidad = 1)
    {
        if (string.IsNullOrEmpty(_carritoId))
            return false;

        var response = await _httpClient.DeleteAsync($"/carritos/{_carritoId}/{productoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VaciarCarritoAsync()
    {
        if (string.IsNullOrEmpty(_carritoId))
            return false;

        var response = await _httpClient.DeleteAsync($"/carritos/{_carritoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<CompraResponse?> ConfirmarCompraAsync(DatosCliente datosCliente)
    {
        if (string.IsNullOrEmpty(_carritoId))
            return null;

        var response = await _httpClient.PutAsJsonAsync($"/carritos/{_carritoId}/confirmar", datosCliente);
        
        if (response.IsSuccessStatusCode)
        {
            var compraResponse = await response.Content.ReadFromJsonAsync<CompraResponse>();
            _carritoId = null; // Reset cart after successful purchase
            return compraResponse;
        }
        
        return null;
    }

    public int GetCantidadItemsCarrito(List<ItemCarrito> items)
    {
        return items.Sum(i => i.Cantidad);
    }

    public decimal GetTotalCarrito(List<ItemCarrito> items)
    {
        return items.Sum(i => i.Importe);
    }
}