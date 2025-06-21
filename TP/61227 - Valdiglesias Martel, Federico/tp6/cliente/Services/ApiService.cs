using System.Net.Http.Json;
using cliente.modelos;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly EstadoCarrito _estadoCarrito;

    private List<Producto>? _catalogoCache;

    public event Action? OnCatalogoChange;

    public ApiService(HttpClient http, EstadoCarrito estado)
    {
        _http = http;
        _estadoCarrito = estado;
    }

    public async Task<List<Producto>?> ObtenerProductosAsync(string? q = null)
    {
        if (!string.IsNullOrWhiteSpace(q))
        {
            return await _http.GetFromJsonAsync<List<Producto>>($"api/productos?q={q}");
        }

        if (_catalogoCache == null)
        {
            _catalogoCache = await _http.GetFromJsonAsync<List<Producto>>("api/productos");
        }
        return _catalogoCache;
    }

    public async Task AgregarAlCarrito(int productoId)
    {
        if (_estadoCarrito.CarritoId == null)
        {
            var res = await _http.PostAsync("api/compras", null);
            if (res.IsSuccessStatusCode)
            {
                var nuevaCompra = await res.Content.ReadFromJsonAsync<CompraRespuestaDto>();
                if (nuevaCompra != null) _estadoCarrito.SetCarritoId(nuevaCompra.Id);
            }
            else return;
        }

        var respuesta = await _http.PutAsync($"api/compras/{_estadoCarrito.CarritoId}/productos/{productoId}", null);
        if (respuesta.IsSuccessStatusCode)
        {
            ActualizarStockLocal(productoId, -1);
            await ActualizarConteoCarritoAsync();
        }
    }

    public async Task EliminarDelCarrito(int productoId)
    {
        if (_estadoCarrito.CarritoId == null) return;

        var respuesta = await _http.DeleteAsync($"api/compras/{_estadoCarrito.CarritoId}/productos/{productoId}");
        if (respuesta.IsSuccessStatusCode)
        {
            ActualizarStockLocal(productoId, 1);
            await ActualizarConteoCarritoAsync();
        }
    }

    public async Task VaciarCarritoCompletoAsync()
    {
        if (_estadoCarrito.CarritoId == null) return;

        var carrito = await GetCarritoAsync();
        if (carrito == null) return;

        var respuesta = await _http.DeleteAsync($"api/compras/{_estadoCarrito.CarritoId}");
        if (respuesta.IsSuccessStatusCode)
        {
            foreach (var item in carrito.Items)
            {
                ActualizarStockLocal(item.ProductoId, item.Cantidad);
            }
            _estadoCarrito.LimpiarCarrito();
        }
    }

    public async Task<CompraRespuestaDto?> GetCarritoAsync()
    {
        if (_estadoCarrito.CarritoId == null) return null;
        return await _http.GetFromJsonAsync<CompraRespuestaDto>($"api/compras/{_estadoCarrito.CarritoId}");
    }

    public async Task<CompraRespuestaDto?> ConfirmarCompraAsync(DatosClienteDto cliente)
    {
        if (_estadoCarrito.CarritoId == null) return null;
        var res = await _http.PutAsJsonAsync($"api/compras/{_estadoCarrito.CarritoId}/confirmar", cliente);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<CompraRespuestaDto>() : null;
    }

    private async Task ActualizarConteoCarritoAsync()
    {
        var carrito = await GetCarritoAsync();
        _estadoCarrito.SetItemsEnCarrito(carrito?.Items.Sum(i => i.Cantidad) ?? 0);
    }

    private void ActualizarStockLocal(int productoId, int cantidad)
    {
        if (_catalogoCache != null)
        {
            var productoAfectado = _catalogoCache.FirstOrDefault(p => p.Id == productoId);
            if (productoAfectado != null)
            {
                productoAfectado.Stock += cantidad;
                OnCatalogoChange?.Invoke();
            }
        }
    }
}