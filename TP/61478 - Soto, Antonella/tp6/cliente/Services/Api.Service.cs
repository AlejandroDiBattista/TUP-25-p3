using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using cliente.Models;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public ApiService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<List<Producto>> ObtenerProductos(string? filtro)
        {
            string url = "/productos";
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                url += $"?nombre={Uri.EscapeDataString(filtro)}";
            }

            return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new List<Producto>();
        }

        public async Task<int> ObtenerOCrearCarritoIdAsync()
        {
            var carritoIdJson = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");

            if (int.TryParse(carritoIdJson, out var carritoId))
            {
                return carritoId;
            }

            var response = await _http.PostAsync("/carritos", null);
            if (!response.IsSuccessStatusCode)
                throw new Exception("No se pudo crear el carrito.");

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            carritoId = doc.RootElement.GetProperty("id").GetInt32();

            await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", carritoId.ToString());

            return carritoId;
        }

        public async Task AgregarAlCarrito(int productoId, int cantidad)
        {
            var carritoId = await ObtenerOCrearCarritoIdAsync();
            var url = $"/carritos/{carritoId}/{productoId}?cantidad={cantidad}";
            var response = await _http.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al agregar al carrito: {error}");
            }
        }

        public async Task<Carrito?> ObtenerCarrito()
        {
            var carritoId = await ObtenerOCrearCarritoIdAsync();
            var url = $"/carritos/{carritoId}";
            return await _http.GetFromJsonAsync<Carrito>(url);
        }

        public async Task ConfirmarCompra(int carritoId, object datos)
        {
            var response = await _http.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al confirmar la compra: {error}");
            }

            await _js.InvokeVoidAsync("localStorage.removeItem", "carritoId");
        }
    }
}
