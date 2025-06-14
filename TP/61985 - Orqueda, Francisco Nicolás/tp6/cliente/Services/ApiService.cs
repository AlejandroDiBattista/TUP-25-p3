using System.Net.Http.Json;
using cliente.Models;
using System.Net.Http;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
        public async Task<List<Compra>> ObtenerComprasAsync()
        {
            try
            {
                var compras = await _httpClient.GetFromJsonAsync<List<Compra>>("/api/compras");
                return compras ?? new List<Compra>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener compras: {ex.Message}");
                return new List<Compra>();
            }
        }
        public async Task<bool> AgregarCompraAsync(Compra nuevaCompra)
        {
            try
            {
                var respuesta = await _httpClient.PostAsJsonAsync("/api/compras", nuevaCompra);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar compra: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> EliminarCompraAsync(int id)
        {
            try
            {
                var respuesta = await _httpClient.DeleteAsync($"/api/compras/{id}");
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar compra: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
        {
            try
            {
                var url = "/productos";
                if (!string.IsNullOrWhiteSpace(busqueda))
                    url += $"?q={busqueda}";
                var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
                return productos ?? new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        // Crear un nuevo carrito y obtener su Id
        public async Task<Guid?> CrearCarritoAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("/carritos", null);
                if (response.IsSuccessStatusCode)
                {
                    var carritoId = await response.Content.ReadFromJsonAsync<Guid>();
                    return carritoId;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear carrito: {ex.Message}");
                return null;
            }
        }

        // Obtener los items de un carrito
        public async Task<List<CarritoItem>> ObtenerItemsCarritoAsync(Guid carritoId)
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<CarritoItem>>($"/carritos/{carritoId}");
                return items ?? new List<CarritoItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener items del carrito: {ex.Message}");
                return new List<CarritoItem>();
            }
        }

        // Agregar o actualizar producto en carrito
        public async Task<bool> AgregarProductoAlCarritoAsync(Guid carritoId, int productoId, int cantidad)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/{productoId}", cantidad);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar producto al carrito: {ex.Message}");
                return false;
            }
        }

        // Vaciar carrito
        public async Task<bool> VaciarCarritoAsync(Guid carritoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al vaciar carrito: {ex.Message}");
                return false;
            }
        }

        // Confirmar compra
        public async Task<bool> ConfirmarCompraAsync(Guid carritoId, CompraConfirmacionDto datos)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al confirmar compra: {ex.Message}");
                return false;
            }
        }
    }

    public class DatosRespuesta
    {
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}