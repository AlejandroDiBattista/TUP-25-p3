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

    public async Task<List<Producto>> ObtenerProductosAsync(string buscar = null)
    {
        var url = "/productos";
        if (!string.IsNullOrEmpty(buscar))
            url += $"?buscar={buscar}";
        return await _httpClient.GetFromJsonAsync<List<Producto>>(url);
    }

    public async Task<string> CrearCarritoAsync()
    {
        var resp = await _httpClient.PostAsync("/carritos", null);
        return await resp.Content.ReadAsStringAsync();
    }

    public async Task<List<CarritoItem>> ObtenerCarritoAsync(string carritoId)
    {
        return await _httpClient.GetFromJsonAsync<List<CarritoItem>>($"/carritos/{carritoId}");
    }

    public async Task VaciarCarritoAsync(string carritoId)
    {
        await _httpClient.DeleteAsync($"/carritos/{carritoId}");
    }

    public async Task AgregarOActualizarProductoEnCarritoAsync(string carritoId, int productoId, int cantidad)
    {
        var url = $"/carritos/{carritoId}/{productoId}?cantidad={cantidad}";
        await _httpClient.PutAsync(url, null);
    }

    public async Task EliminarProductoDelCarritoAsync(string carritoId, int productoId)
    {
        await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
    }

    public async Task<ConfirmacionCompraRespuesta> ConfirmarCompraAsync(string carritoId, ClienteDatos datos)
    {
        var resp = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
        return await resp.Content.ReadFromJsonAsync<ConfirmacionCompraRespuesta>();
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
