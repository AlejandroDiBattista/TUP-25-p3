using System.Net.Http.Json;
using cliente.Dtos;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private int carritoId = 0;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
        {
            try
            {
                string url = "/productos";
                if (!string.IsNullOrWhiteSpace(busqueda))
                    url += $"?q={System.Net.WebUtility.UrlEncode(busqueda)}";
                var productos = await _httpClient.GetFromJsonAsync<List<Producto>>(url);
                return productos ?? new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        public async Task<int> GetOrCreateCarritoIdAsync()
        {
            if (carritoId == 0)
            {
                var response = await _httpClient.PostAsync("/carritos", null);
                if (response.IsSuccessStatusCode)
                {
                    var idString = await response.Content.ReadAsStringAsync();
                    if (int.TryParse(idString, out int id))
                        carritoId = id;
                    else
                    {
                        try
                        {
                            var obj = System.Text.Json.JsonDocument.Parse(idString);
                            if (obj.RootElement.TryGetProperty("value", out var val))
                                carritoId = val.GetInt32();
                        }
                        catch { }
                    }
                }
            }
            return carritoId;
        }

        public async Task<bool> AgregarProductoAlCarritoAsync(Producto producto)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.PutAsync($"/carritos/{id}/{producto.Id}?cantidad=1", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CambiarCantidadProductoAsync(int productoId, int cantidad)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.PutAsync($"/carritos/{id}/{productoId}?cantidad={cantidad}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarProductoDelCarritoAsync(int productoId)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.DeleteAsync($"/carritos/{id}/{productoId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VaciarCarritoAsync()
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.DeleteAsync($"/carritos/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool exito, string mensaje)> ConfirmarCompraAsync(object cliente)
        {
            int id = await GetOrCreateCarritoIdAsync();
            var response = await _httpClient.PutAsJsonAsync($"/carritos/{id}/confirmar", cliente);
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "Compra confirmada correctamente" : "Error al confirmar la compra");
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("mensaje", out var msgProp))
                {
                    return (response.IsSuccessStatusCode, msgProp.GetString() ?? (response.IsSuccessStatusCode ? "Compra confirmada correctamente" : "Error al confirmar la compra"));
                }
            }
            catch { }
           
            return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "Compra confirmada correctamente" : "Error al confirmar la compra");
        }

        public async Task<List<CompraDto>> ObtenerComprasAsync()
        {
            try
            {
                var compras = await _httpClient.GetFromJsonAsync<List<CompraDto>>("/compras");
                return compras ?? new List<CompraDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener compras: {ex.Message}");
                return new List<CompraDto>();
            }
        }

        public async Task ResetCarritoIdAsync()
        {
            carritoId = 0;
        }
    }


    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }
}
