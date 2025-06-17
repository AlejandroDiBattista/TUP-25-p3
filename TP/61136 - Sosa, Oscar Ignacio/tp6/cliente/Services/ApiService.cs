using System.Net.Http.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    // GET /productos (+ búsqueda por query)
    public async Task<List<Producto>> ObtenerProductosAsync(string? query = null) {
        try {
            string url = "/api/productos";
            if (!string.IsNullOrEmpty(query))
                url += $"?q={Uri.EscapeDataString(query)}";
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
            return productos ?? new List<Producto>();
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }

    // POST /carritos (inicializa el carrito)
    public async Task<CarritoRespuesta?> CrearCarritoAsync() {
        try {
            var response = await _httpClient.PostAsJsonAsync("/api/carritos", new { });
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CarritoRespuesta>();
        } catch (Exception ex) {
            Console.WriteLine($"Error al crear carrito: {ex.Message}");
        }
        return null;
    }

    // GET /carritos/{carrito}
    public async Task<Carrito?> ObtenerCarritoAsync(string carritoId) {
        try {
            return await _httpClient.GetFromJsonAsync<Carrito>($"/api/carritos/{carritoId}");
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener carrito: {ex.Message}");
            return null;
        }
    }

    // DELETE /carritos/{carrito}
    public async Task<bool> VaciarCarritoAsync(string carritoId) {
        try {
            var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Console.WriteLine($"Error al vaciar carrito: {ex.Message}");
            return false;
        }
    }

    // PUT /carritos/{carrito}/confirmar
    public async Task<bool> ConfirmarCompraAsync(string carritoId, ConfirmacionDatos datos) {
        try {
            var response = await _httpClient.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", datos);
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Console.WriteLine($"Error al confirmar compra: {ex.Message}");
            return false;
        }
    }

    // PUT /carritos/{carrito}/{producto}
    public async Task<bool> AgregarOActualizarProductoAsync(string carritoId, int productoId, int cantidad) {
        try {
            var response = await _httpClient.PutAsJsonAsync(
                $"/api/carritos/{carritoId}/{productoId}",
                new { cantidad }
            );
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Console.WriteLine($"Error al agregar/actualizar producto: {ex.Message}");
            return false;
        }
    }

    // DELETE /carritos/{carrito}/{producto}
    public async Task<bool> EliminarProductoAsync(string carritoId, int productoId) {
        try {
            var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Console.WriteLine($"Error al eliminar producto: {ex.Message}");
            return false;
        }
    }

    // Ejemplo para obtener datos simples (ajusta según tu API)
    public async Task<DatosRespuesta?> ObtenerDatosAsync()
    {
        return await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
    }
}

// Modelos de ejemplo
public class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public class Carrito {
    public string Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new();
}

public class ItemCarrito {
    public int ProductoId { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}

public class CarritoRespuesta {
    public string Id { get; set; }
}

public class ConfirmacionDatos {
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    // Puedes agregar más campos si tu API lo requiere
}

// Modelo para la respuesta de datos simples (ajusta según tu API)
public class DatosRespuesta
{
    public string Info { get; set; }
    // Agrega más propiedades según la estructura de tu respuesta
}
