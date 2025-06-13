using System.Net.Http.Json;
using cliente.Models;

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
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            try
            {
                var productos = await _httpClient.GetFromJsonAsync<List<Producto>>("/api/productos");
                return productos ?? new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }
        public async Task RegistrarCompraAsync(CompraDTO compra)
        {
            await _httpClient.PostAsJsonAsync("api/compras", compra);
        }
    }

}

    public class CompraDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public List<ItemCompraDTO> Items { get; set; }
    }

    public class ItemCompraDTO
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }

    public class DatosRespuesta
    {
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
