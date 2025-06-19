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

    
    public async Task<List<Producto>> ObtenerProductosAsync(string filtro = null)
    {
        var url = "/productos";
        if (!string.IsNullOrEmpty(filtro))
        {
            url += $"?filtro={filtro}";
        }
        var resp = await _httpClient.GetAsync(url);

        if (resp.IsSuccessStatusCode)
        {
            var productos = await resp.Content.ReadFromJsonAsync<List<Producto>>();
            return productos;
        }
        else
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    
    public async Task<string> CrearCarritoAsync()
    {
        var resp = await _httpClient.PostAsync("/carritos", null);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        return await resp.Content.ReadAsStringAsync();
    }

    
    public async Task<List<ProductoCarrito>> ObtenerCarritoAsync(string carritoId)
    {
        var resp = await _httpClient.GetAsync($"/carritos/{carritoId}");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
        return await resp.Content.ReadFromJsonAsync<List<ProductoCarrito>>();
    }

    
    public async Task VaciarCarritoAsync(string carritoId)
    {
        var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    
    public async Task AgregarOActualizarProductoEnCarritoAsync(string carritoId, int productoId, int cantidad)
    {
        var url = $"/carritos/{carritoId}/{productoId}?cantidad={cantidad}";
        var resp = await _httpClient.PutAsync(url, null);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

   
    public async Task EliminarProductoDelCarritoAsync(string carritoId, int productoId, int cantidad)
    {
        var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    
    public async Task<ConfirmacionCompraRespuesta> ConfirmarCompraAsync(string carritoId, ClienteDatos datos)
    {
        var resp = await _httpClient.PostAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
        return await resp.Content.ReadFromJsonAsync<ConfirmacionCompraRespuesta>();
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
