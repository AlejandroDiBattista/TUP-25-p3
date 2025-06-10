using System.Net.Http.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;
public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    // Obtener productos (con búsqueda opcional)
    public async Task<List<ProductoDto>> GetProductosAsync(string? q = null)
    {
        var url = "/api/productos";
        if (!string.IsNullOrWhiteSpace(q))
            url += $"?q={Uri.EscapeDataString(q)}";
        return await _http.GetFromJsonAsync<List<ProductoDto>>(url) ?? new();
    }

    // Crear un nuevo carrito
    public async Task<Guid> CrearCarritoAsync()
    {
        var resp = await _http.PostAsync("/api/carritos", null);
        var data = await resp.Content.ReadFromJsonAsync<CarritoIdDto>();
        return data?.carritoId ?? Guid.Empty;
    }

    // Obtener los ítems del carrito
    public async Task<List<CarritoItemDto>> GetCarritoAsync(Guid carritoId)
    {
        return await _http.GetFromJsonAsync<List<CarritoItemDto>>($"/api/carritos/{carritoId}") ?? new();
    }

    // Agregar o actualizar producto en el carrito
    public async Task<bool> AgregarProductoAlCarritoAsync(Guid carritoId, int productoId, int cantidad)
    {
        var resp = await _http.PutAsJsonAsync($"/api/carritos/{carritoId}/{productoId}", new { cantidad });
        return resp.IsSuccessStatusCode;
    }

    // Eliminar producto del carrito
    public async Task<bool> EliminarProductoDelCarritoAsync(Guid carritoId, int productoId)
    {
        var resp = await _http.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
        return resp.IsSuccessStatusCode;
    }

    // Vaciar carrito
    public async Task<bool> VaciarCarritoAsync(Guid carritoId)
    {
        var resp = await _http.DeleteAsync($"/api/carritos/{carritoId}");
        return resp.IsSuccessStatusCode;
    }

    // Confirmar compra
    public async Task<bool> ConfirmarCompraAsync(Guid carritoId, ConfirmarCompraDto datos)
    {
        var resp = await _http.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", datos);
        return resp.IsSuccessStatusCode;
    }
}

// --- DTOs para el cliente ---
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = "";
}

public class CarritoIdDto
{
    public Guid carritoId { get; set; }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
public class CarritoItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal Subtotal { get; set; }
}

public class ConfirmarCompraDto
{
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Email { get; set; } = "";
}
