using System.Net.Http.Json;
using Microsoft.JSInterop;
using tp6.Models;
using System.Text.Json;

namespace Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private Guid carritoId;

        public ApiService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
            _http.BaseAddress = new Uri("http://localhost:5184"); // Asegurate que coincida con tu backend
        }

        // Obtener productos, con parámetro opcional de búsqueda
        public async Task<List<Producto>> ObtenerProductosAsync(string? busqueda = null)
        {
            try
            {
                string url = "/productos";
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    url += $"?q={Uri.EscapeDataString(busqueda)}";
                }

                var productos = await _http.GetFromJsonAsync<List<Producto>>(url);
                return productos ?? new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        // Obtener o crear carrito, guardando el Id en memoria y localStorage
        public async Task<Guid> ObtenerOCrearCarritoAsync()
        {
            if (carritoId != Guid.Empty)
                return carritoId;

            var response = await _http.PostAsync("/carrito", null);
            response.EnsureSuccessStatusCode();
            carritoId = await response.Content.ReadFromJsonAsync<Guid>();
            await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", carritoId.ToString());
            return carritoId;
        }

        // Obtener ítems del carrito con información enriquecida (Nombre, Precio, Subtotal)
        public async Task<List<CarritoItem>> ObtenerCarritoItemsAsync()
        {
            await ObtenerOCrearCarritoAsync();

            var response = await _http.GetAsync($"/carrito/{carritoId}/items");
            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<List<CarritoItem>>();
                return items ?? new List<CarritoItem>();
            }

            return new List<CarritoItem>();
        }

        // Agregar o actualizar cantidad de un producto en el carrito
        public async Task<bool> AgregarAlCarritoAsync(Guid carritoId, int productoId, int cantidad)
        {
            var response = await _http.PutAsync($"/carrito/{carritoId}/{productoId}?cantidad={cantidad}", null);
            return response.IsSuccessStatusCode;
        }

        // Vaciar carrito
        public async Task VaciarCarritoAsync()
        {
            await ObtenerOCrearCarritoAsync();
            await _http.DeleteAsync($"/carrito/{carritoId}");
        }

        // Eliminar producto específico del carrito
        public async Task<bool> EliminarItemCarritoAsync(int productoId)
        {
            await ObtenerOCrearCarritoAsync();
            var response = await _http.DeleteAsync($"/carrito/{carritoId}/{productoId}");
            return response.IsSuccessStatusCode;
        }

        // Confirmar compra con datos del cliente
        public async Task<bool> ConfirmarCompraAsync(string nombre, string apellido, string email)
        {
            await ObtenerOCrearCarritoAsync();

            var datos = new
            {
                NombreCliente = nombre,
                ApellidoCliente = apellido,
                EmailCliente = email
            };

            var response = await _http.PutAsJsonAsync($"/carrito/{carritoId}/confirmar", datos);
            if (response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", "carritoId");
                carritoId = Guid.Empty;
                return true;
            }

            return false;
        }

        // Obtener cantidad total de items en carrito
        public async Task<int> ObtenerCantidadCarritoAsync()
        {
            await ObtenerOCrearCarritoAsync();
            return await _http.GetFromJsonAsync<int>($"/carrito/{carritoId}/cantidad");
        }
    }
}
