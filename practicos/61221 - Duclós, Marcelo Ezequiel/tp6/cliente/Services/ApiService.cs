using System.Net.Http;
using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private List<Producto> _productosCache;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_productosCache != null)
                    return _productosCache;

                _productosCache = await _httpClient.GetFromJsonAsync<List<Producto>>("/api/productos") ?? new List<Producto>();
                return _productosCache;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> RegistrarVentaAsync(RegistrarVentaRequest ventaRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/ventas", ventaRequest);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al registrar venta: {content}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegistrarVentaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<VentaResponse>> ObtenerHistorialAsync(string email)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/ventas/historial/{email.Trim()}");
                var content = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Respuesta del servidor: {content}");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obtener historial: {content}");
                    return new List<VentaResponse>();
                }

                var ventas = await response.Content.ReadFromJsonAsync<List<VentaResponse>>();
                return ventas ?? new List<VentaResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerHistorialAsync: {ex.Message}");
                return new List<VentaResponse>();
            }
        }

        public class Cliente
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
        }
    }
}
