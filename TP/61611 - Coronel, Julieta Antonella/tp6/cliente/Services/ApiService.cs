using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using cliente.Models;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        public ApiService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5083/");
            _localStorageService = localStorageService;
        }

        private async Task InicializarCarrito(string carritoId)
        {
            if (string.IsNullOrEmpty(await _localStorageService.GetItemAsStringAsync("carrito")))
                await _localStorageService.SetItemAsStringAsync("carrito", carritoId);
        }

        public async Task<IList<Producto>> ObtenerProductos(string query = "")
        {
            try
            {
                IList<Producto> productos = await _httpClient.GetFromJsonAsync<IList<Producto>>($"api/productos?parametroBusqueda={query}");
                return productos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return null;
            }
        }

        public async Task<DatosRespuesta> ObtenerDatosAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("api/datos");
                return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
            }
        }

        public async Task<string> NuevoProducto(Producto producto)
        {
            try
            {
                var content = new StringContent(producto.ToString(), Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync("api/nuevoProducto", content);
                // string json = JsonSerializer.Serialize(producto);
                // _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                // var result = await _httpClient.PostAsJsonAsync("https://localhost:7060/api/nuevoProducto", json);
                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadAsStringAsync();
                }
                else
                    return "Error al cargar producto.";
            }
            catch
            {
                return "error";
            }
        }

        public async Task AgregarAlCarrito(int productoId)
        {
            // Obtener productos para validar stock
            var productos = await ObtenerProductos("");
            var producto = productos?.FirstOrDefault(p => p.Id == productoId);
            if (producto == null || producto.Stock == 0)
            {
                // No permitir agregar si no hay stock
                return;
            }
            string carritoId = await _localStorageService.GetItemAsync<string>("carrito");
            if (string.IsNullOrEmpty(carritoId))
            {
                carritoId = await ObtenerIdCarrito();
                await _localStorageService.SetItemAsync("carrito", carritoId);
            }
            await _httpClient.GetAsync($"api/carritos/{carritoId}/{productoId}");
        }

        public async Task AgregarAlCarrito(int productoId, int stock)
        {
            if (stock <= 0)
                return;
            string carritoId = await _localStorageService.GetItemAsync<string>("carrito");
            if (string.IsNullOrEmpty(carritoId))
            {
                carritoId = await ObtenerIdCarrito();
                await _localStorageService.SetItemAsync("carrito", carritoId);
            }
            await _httpClient.GetAsync($"api/carritos/{carritoId}/{productoId}");
        }

        public async Task<string> ObtenerIdCarrito()
        {
            return await _httpClient.GetStringAsync("api/carritos");
        }

        public async Task<IList<Producto>> ObtenerCarrito()
        {
            string carritoId = await _localStorageService.GetItemAsync<string>("carrito");

            // Si no hay carrito guardado, crear uno
            if (string.IsNullOrEmpty(carritoId))
            {
                carritoId = await ObtenerIdCarrito(); // esto llama a api/carritos y crea uno
                await _localStorageService.SetItemAsync("carrito", carritoId);
            }

            try
            {
                IList<Producto> productos = await _httpClient.GetFromJsonAsync<IList<Producto>>($"api/carritos/{carritoId}");
                return productos;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener el carrito: {ex.Message}");
                return new List<Producto>();
            }
        }


        public async Task<List<CarritoDto>> ObtenerCarritoDetalle()
        {
            var carritoId = await _localStorageService.GetItemAsync<string>("carrito");

            if (string.IsNullOrEmpty(carritoId))
            {
                carritoId = await ObtenerIdCarrito();
                await _localStorageService.SetItemAsync("carrito", carritoId);
            }

            try
            {
                return await _httpClient.GetFromJsonAsync<List<CarritoDto>>($"api/carritos/{carritoId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener carrito con detalle: {ex.Message}");
                return new List<CarritoDto>();
            }
        }

        public async Task<string> NuevaCompra(NuevaCompraDto nuevaCompraDto)
        {
            string carritoId = await _localStorageService.GetItemAsync<string>("carrito");
            nuevaCompraDto.CarritoId = carritoId;

            var response = await _httpClient.PostAsJsonAsync("api/nuevaCompra", nuevaCompraDto);

            if (response.IsSuccessStatusCode)
            {
                await ObtenerProductos(); // ✔️ Lo movés arriba
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al confirmar compra: {error}");
                return $"Error: {response.StatusCode}";
            }
        }



        public async Task DisminuirCantidadProducto(int productoId)
        {
            var carritoId = await _localStorageService.GetItemAsync<string>("carrito");
            await _httpClient.PutAsync($"api/carritos/{carritoId}/disminuir/{productoId}", null);
        }

        public async Task EliminarProductoDelCarrito(int productoId)
        {
            var carritoId = await _localStorageService.GetItemAsync<string>("carrito");
            await _httpClient.DeleteAsync($"api/carritos/{carritoId}/eliminar/{productoId}");
        }

        public async Task VaciarCarrito()
        {
            var carritoId = await _localStorageService.GetItemAsync<string>("carrito");
            await _httpClient.DeleteAsync($"api/carritos/{carritoId}/vaciar");
        }
        
        

    }

    public class DatosRespuesta
    {
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}