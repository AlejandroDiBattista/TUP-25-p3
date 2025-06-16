using System.Net.Http.Json;
using Cliente.Modelos;

namespace Cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient http;

        public ApiService(HttpClient http)
        {
            this.http = http;
            this.http.BaseAddress = new Uri("http://localhost:5184/");
        }
          public async Task<List<Producto>> ObtenerProductosAsync()
        {
            var productos = await http.GetFromJsonAsync<List<Producto>>("http://localhost:5177/productos");
            return productos ?? new List<Producto>();
        }
        public async Task<bool> AgregarAlCarrito(Guid carritoId, int productoId, int cantidad)
        {
            try
            {
                var response = await http.PutAsJsonAsync(
                    $"/carritos/{carritoId}/{productoId}", cantidad);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al agregar al carrito: {ex.Message}");
                return false;
            }
        }
        public async Task<DatosRespuesta> ObtenerDatosAsync()
        {
            try
            {
                var respuesta = await http.GetFromJsonAsync<DatosRespuesta>("/api/datos");
                return respuesta ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
            }
        }
       public async Task<List<Producto>> BuscarProductos(string filtro)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(filtro)
                    ? "productos" 
                    : $"productos?q={Uri.EscapeDataString(filtro)}";

                Console.WriteLine($"🔍 URL de búsqueda: {url}");

                var response = await http.GetFromJsonAsync<List<Producto>>(url);

                return response ?? new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }
        public async Task<Guid> CrearCarritoAsync()
        {
            try
            {
                var response = await http.PostAsync("/carritos", null);
                if (response.IsSuccessStatusCode)
                {
                    var id = await response.Content.ReadFromJsonAsync<Guid>();
                    return id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear carrito: {ex.Message}");
            }

            return Guid.Empty;
        }
    }
    public class DatosRespuesta
    {
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}