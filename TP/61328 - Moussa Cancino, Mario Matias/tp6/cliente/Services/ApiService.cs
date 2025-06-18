using System.Net.Http.Json;
using cliente.Modelos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient) {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReferenceHandler = ReferenceHandler.Preserve };
        }

        public async Task<List<Producto>> GetProductosAsync(string? searchQuery = null) {
            var url = $"http://localhost:5184/api/productos{(string.IsNullOrWhiteSpace(searchQuery) ? "" : $"?q={searchQuery}")}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { throw new ApplicationException($"Error de la API: {response.ReasonPhrase}"); }
            return JsonSerializer.Deserialize<List<Producto>>(content, _jsonOptions);
        }

        public async Task<Guid> CreateCartAsync() {
            var response = await _httpClient.PostAsync("http://localhost:5184/api/carritos", null);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CartCreationResponse>();
            return result.CarritoId;
        }

        public async Task AddProductToCartAsync(Guid cartId, int productId) {
            var response = await _httpClient.PutAsync($"http://localhost:5184/api/carritos/{cartId}/productos/{productId}", null);
            response.EnsureSuccessStatusCode();
        }

        // <<< MÉTODO NUEVO >>>
        public async Task RemoveProductFromCartAsync(Guid cartId, int productId) {
            var response = await _httpClient.DeleteAsync($"http://localhost:5184/api/carritos/{cartId}/productos/{productId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task EmptyCartAsync(Guid cartId) {
            var response = await _httpClient.DeleteAsync($"http://localhost:5184/api/carritos/{cartId}");
            response.EnsureSuccessStatusCode();
        }
        
        public async Task ConfirmPurchaseAsync(Guid cartId, DatosCliente datosCliente) {
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5184/api/carritos/{cartId}/confirmar", datosCliente);
            response.EnsureSuccessStatusCode();
        }
    }

    public class CartCreationResponse { public Guid CarritoId { get; set; } }
}