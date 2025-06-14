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
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }

    
    public async Task<List<CarritoItem>> ObtenerCarritoAsync(string carritoId)
    {
        var resp = await _httpClient.GetAsync($"/carritos/{carritoId}");
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<List<CarritoItem>>();
    }

    
    public async Task VaciarCarritoAsync(string carritoId)
    {
        var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        resp.EnsureSuccessStatusCode();
    }

    
    public async Task AgregarOActualizarProductoEnCarritoAsync(string carritoId, int productoId, int cantidad)
    {
        var url = $"/carritos/{carritoId}/{productoId}?cantidad={cantidad}";
        var resp = await _httpClient.PutAsync(url, null);
        resp.EnsureSuccessStatusCode();
    }

   
    public async Task EliminarProductoDelCarritoAsync(string carritoId, int productoId)
    {
        var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
        resp.EnsureSuccessStatusCode();
    }

    
    public async Task<ConfirmacionCompraRespuesta> ConfirmarCompraAsync(string carritoId, ClienteDatos datos)
    {
        var resp = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<ConfirmacionCompraRespuesta>();
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
