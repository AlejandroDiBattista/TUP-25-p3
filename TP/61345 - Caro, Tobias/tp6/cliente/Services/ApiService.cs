using System.Net.Http.Json;
using System.Text.Json;
using cliente.Shared;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
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

    public async Task<List<Producto>> ObtenerProductosAsync() {
        var response = await _httpClient.GetAsync("productos");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Producto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task AgregarAlCarrito(int carritoId, int productoId, int cantidad)
    {
        var response = await _httpClient.PutAsync(
            $"carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ItemCompra>> ObtenerItemsCarritoAsync(int carritoId)
    {
        var response = await _httpClient.GetAsync($"carritos/{carritoId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var carrito = JsonSerializer.Deserialize<CompraCarritoRespuesta>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return carrito?.Items ?? new List<ItemCompra>();
    }

    public async Task<int> InicializarCarritoAsync()
    {
        var response = await _httpClient.PostAsync("carritos", null);
        response.EnsureSuccessStatusCode();
        var idString = await response.Content.ReadAsStringAsync();
        // El backend responde con el id como número, o como JSON ("1"). Intentamos parsear ambos.
        if (int.TryParse(idString, out int id))
            return id;
        // Si viene como JSON: { "value": 1 }
        try
        {
            using var doc = JsonDocument.Parse(idString);
            if (doc.RootElement.TryGetProperty("value", out var valueProp))
                return valueProp.GetInt32();
        }
        catch { }
        throw new Exception("No se pudo obtener el id del carrito");
    }

    public async Task EliminarDelCarrito(int carritoId, int productoId)
    {
        var response = await _httpClient.DeleteAsync($"carritos/{carritoId}/{productoId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<int> ObtenerCantidadCarritoAsync(int carritoId)
    {
        var response = await _httpClient.GetAsync($"carritos/{carritoId}/cantidad");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return -1; // Indicador especial para carrito inexistente
        response.EnsureSuccessStatusCode();
        var cantidadStr = await response.Content.ReadAsStringAsync();
        if (int.TryParse(cantidadStr, out int cantidad))
            return cantidad;
        // Si viene como JSON: { "value": 3 }
        try
        {
            using var doc = JsonDocument.Parse(cantidadStr);
            if (doc.RootElement.TryGetProperty("value", out var valueProp))
                return valueProp.GetInt32();
        }
        catch { }
        return 0;
    }

    public async Task ConfirmarCompraAsync(int carritoId, object datosCliente)
    {
        var response = await _httpClient.PutAsJsonAsync($"carritos/{carritoId}/confirmar", datosCliente);
        response.EnsureSuccessStatusCode();
    }

    public async Task VaciarCarritoAsync(int carritoId)
    {
        var response = await _httpClient.DeleteAsync($"carritos/{carritoId}");
        response.EnsureSuccessStatusCode();
    }

    public class CompraCarritoRespuesta
    {
        public int Id { get; set; }
        public List<ItemCompra> Items { get; set; }
    }

    public class DatosRespuesta {
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}
