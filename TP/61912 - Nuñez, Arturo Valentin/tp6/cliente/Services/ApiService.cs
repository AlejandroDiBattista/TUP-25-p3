using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;  // Para futuras validaciones

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        public ApiService(HttpClient httpClient) => _httpClient = httpClient;

        // Evento para notificar cambios en el carrito (actualizar contador en el UI)
        public event Action CartChanged;

        // 0) Endpoint de prueba
        public async Task<DatosRespuesta> ObtenerDatosAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
                return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
            }
        }

        // 1) Listar productos (con búsqueda opcional)
        public async Task<List<Product>> GetProductosAsync(string q = null)
        {
            var uri = "/productos" + (string.IsNullOrWhiteSpace(q) ? "" : $"?q={Uri.EscapeDataString(q)}");
            return await _httpClient.GetFromJsonAsync<List<Product>>(uri);
        }

        // 2) Crear carrito (POST /carritos)
        public async Task<int> CrearCarritoAsync()
        {
            var resp = await _httpClient.PostAsJsonAsync("/carritos", new { });
            resp.EnsureSuccessStatusCode();
            var obj = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return obj.GetProperty("carritoId").GetInt32();
        }

        // 3) Obtener carrito existente (GET /carritos/{id})
        public async Task<CarritoDto> GetCarritoAsync(int carritoId)
            => await _httpClient.GetFromJsonAsync<CarritoDto>($"/carritos/{carritoId}");

        // 4) Vaciar carrito (DELETE /carritos/{id})
        public async Task VaciarCarritoAsync(int carritoId)
        {
            var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
            resp.EnsureSuccessStatusCode();
            CartChanged?.Invoke();
        }

        // 5) Agregar o incrementar cantidad (PUT /carritos/{id}/{producto})
        public async Task AddToCarritoAsync(int carritoId, int productoId, int cantidad)
        {
            var resp = await _httpClient.PutAsJsonAsync(
                $"/carritos/{carritoId}/{productoId}",
                new QuantityUpdate(cantidad)
            );
            resp.EnsureSuccessStatusCode();
            CartChanged?.Invoke();
        }

        // 6) Eliminar un ítem del carrito (DELETE /carritos/{id}/{producto})
        public async Task RemoveFromCarritoAsync(int carritoId, int productoId)
        {
            var resp = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
            resp.EnsureSuccessStatusCode();
            CartChanged?.Invoke();
        }

        // 7) Confirmar compra (PUT /carritos/{id}/confirmar)
        public async Task<ConfirmacionDto> ConfirmarCompraAsync(int carritoId, ClienteDto cliente)
        {
            var resp = await _httpClient.PutAsJsonAsync(
                $"/carritos/{carritoId}/confirmar",
                cliente
            );
            resp.EnsureSuccessStatusCode();
            CartChanged?.Invoke();
            return await resp.Content.ReadFromJsonAsync<ConfirmacionDto>();
        }
    }

    // —————————————— DTOs ——————————————

    public class DatosRespuesta
    {
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }

    public record QuantityUpdate(int Cantidad);

    public class ClienteDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        // Campos estéticos (no enviados al servidor)
        [Phone(ErrorMessage = "Teléfono inválido")]
        public string Telefono { get; set; }

        public string Direccion { get; set; }
    }

    public class ItemCarritoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CarritoDto
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public List<ItemCarritoDto> Items { get; set; }
    }

    public class ConfirmacionDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
    }
}
