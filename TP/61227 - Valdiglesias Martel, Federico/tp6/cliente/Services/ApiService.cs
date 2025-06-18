using System.Net.Http.Json;
using cliente.modelos;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _http;
    private readonly EstadoCarrito _estadoCarrito;

    public ApiService(HttpClient http, EstadoCarrito estado) {
        _http = http;
        _estadoCarrito = estado;
    }

    public async Task<List<Producto>?> ObtenerProductosAsync(string? q = null) {
        var url = string.IsNullOrWhiteSpace(q) ? "api/productos" : $"api/productos?q={q}";
        return await _http.GetFromJsonAsync<List<Producto>>(url);
    }

    public async Task AgregarAlCarrito(int productoId) {
        if (_estadoCarrito.CarritoId == null) {
            var res = await _http.PostAsync("api/compras", null);
            if (res.IsSuccessStatusCode) {
                var nuevaCompra = await res.Content.ReadFromJsonAsync<CompraRespuestaDto>();
                if (nuevaCompra != null) _estadoCarrito.SetCarritoId(nuevaCompra.Id);
            } else return;
        }
        await _http.PutAsync($"api/compras/{_estadoCarrito.CarritoId}/productos/{productoId}", null);
        await ActualizarConteoCarritoAsync();
    }

    public async Task<CompraRespuestaDto?> GetCarritoAsync() {
        if (_estadoCarrito.CarritoId == null) return null;
        return await _http.GetFromJsonAsync<CompraRespuestaDto>($"api/compras/{_estadoCarrito.CarritoId}");
    }

    public async Task EliminarDelCarrito(int productoId) {
        if (_estadoCarrito.CarritoId == null) return;
        await _http.DeleteAsync($"api/compras/{_estadoCarrito.CarritoId}/productos/{productoId}");
        await ActualizarConteoCarritoAsync();
    }

    public async Task<CompraRespuestaDto?> ConfirmarCompraAsync(DatosClienteDto cliente) {
        if (_estadoCarrito.CarritoId == null) return null;
        var res = await _http.PutAsJsonAsync($"api/compras/{_estadoCarrito.CarritoId}/confirmar", cliente);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<CompraRespuestaDto>() : null;
    }

    private async Task ActualizarConteoCarritoAsync() {
        var carrito = await GetCarritoAsync();
        _estadoCarrito.SetItemsEnCarrito(carrito?.Items.Sum(i => i.Cantidad) ?? 0);
    }
}