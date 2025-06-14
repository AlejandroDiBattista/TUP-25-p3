#nullable enable
using System.Net.Http.Json;

namespace cliente.Services {
    public class Producto {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }

    public class CarritoItemDto {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class ConfirmacionDto {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

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

        // SERVICIO API
        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null) {
            var url = "/api/productos" + (string.IsNullOrWhiteSpace(busqueda) ? "" : $"?busqueda={busqueda}");
            return await _httpClient.GetFromJsonAsync<List<Producto>>(url) ?? new();
        }

        public async Task<Guid> CrearCarritoAsync() {
            var resp = await _httpClient.PostAsync("/api/carritos", null);
            var id = await resp.Content.ReadFromJsonAsync<Guid>();
            return id;
        }

        public async Task<List<CarritoItemDto>> ObtenerCarritoAsync(Guid carritoId) {
            return await _httpClient.GetFromJsonAsync<List<CarritoItemDto>>($"/api/carritos/{carritoId}") ?? new();
        }

        public async Task AgregarProductoAsync(Guid carritoId, int productoId, int cantidad) {
            await _httpClient.PutAsync($"/api/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        }

        public async Task QuitarProductoAsync(Guid carritoId, int productoId) {
            await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
        }

        public async Task VaciarCarritoAsync(Guid carritoId) {
            await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
        }

        public async Task ConfirmarCompraAsync(Guid carritoId, ConfirmacionDto datos) {
            await _httpClient.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", datos);
        }

        // Setea la cantidad exacta de un producto en el carrito
        public async Task ActualizarCantidadProductoAsync(Guid carritoId, int productoId, int cantidad) {
            await _httpClient.PutAsync($"/api/carritos/{carritoId}/{productoId}?cantidad={cantidad}&set=true", null);
        }
    }

    public class DatosRespuesta {
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
