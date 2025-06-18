using System.Net.Http.Json;
using cliente.Models;
using Microsoft.JSInterop;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        public int CarritoId { get; private set; }

        private int carritoCantidad = 0;

        public CarritoService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task InitializeCarrito()
        {
            // Intentar recuperar el CarritoId del localStorage
            var idString = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");
            int id = 0;
            int.TryParse(idString, out id);

            if (id != 0)
            {
                CarritoId = id;
                return;
            }

            var response = await _http.PostAsync("/carritos", null);
            if (response.IsSuccessStatusCode)
            {
                var carrito = await response.Content.ReadFromJsonAsync<Compra>();
                CarritoId = carrito?.Id ?? 0;
                await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", CarritoId.ToString());
            }
        }

        public async Task<Compra> GetCarritoAsync()
        {
            return await _http.GetFromJsonAsync<Compra>($"/carritos/{CarritoId}") ?? new();
        }

        public async Task AgregarProducto(int productoId, int cantidad)
        {
            await _http.PutAsync($"/carritos/{CarritoId}/{productoId}/{cantidad}", null);
        }

        public async Task VaciarCarrito()
        {
            await _http.DeleteAsync($"/carritos/{CarritoId}");
            CarritoId = 0;
            await _js.InvokeVoidAsync("localStorage.removeItem", "carritoId");
        }

        public async Task ConfirmarCompra(Compra compra)
        {
            await _http.PutAsJsonAsync($"/carritos/{CarritoId}/confirmar", compra);
            CarritoId = 0;
            await _js.InvokeVoidAsync("localStorage.removeItem", "carritoId");
        }
    }
}