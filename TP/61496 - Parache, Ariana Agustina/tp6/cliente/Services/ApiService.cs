using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Models
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task ConfirmarCompra(CompraDto compra)
        {
            // Método alias para registrar compra
            return RegistrarCompraAsync(compra);
        }

        public async Task RegistrarCompraAsync(CompraDto compra)
        {
            // Envía la compra al backend vía POST
            var response = await _httpClient.PostAsJsonAsync("compras", compra);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Producto>?> ObtenerProductosAsync()
        {
            try
            {
                // Eliminar la barra inicial para que tome baseAddress correctamente
                var response = await _httpClient.GetFromJsonAsync<List<Producto>>("productos");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return null;
            }
        }
    }
}
