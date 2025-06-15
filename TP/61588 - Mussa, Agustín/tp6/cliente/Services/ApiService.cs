using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
    {
        string url = "/productos";
        if (!string.IsNullOrWhiteSpace(busqueda))
            url += $"?busqueda={Uri.EscapeDataString(busqueda)}";
        var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
        return productos ?? new List<Producto>();
    }

    public async Task<Carrito> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/carritos", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Carrito>();
    }

    public async Task<Carrito> ObtenerCarritoAsync(int carritoId)
    {
        return await _httpClient.GetFromJsonAsync<Carrito>($"/carritos/{carritoId}");
    }

    public async Task AgregarProductoAlCarrito(int carritoId, int productoId, int cantidad)
    {
        var response = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task VaciarCarritoAsync(int carritoId)
    {
        var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task EliminarProductoDelCarritoAsync(int carritoId, int productoId)
    {
        var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task ConfirmarCompraAsync(int carritoId, CompraConfirmacionDto confirmacion)
    {
        var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", confirmacion);
        response.EnsureSuccessStatusCode();
    }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
