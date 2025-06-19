using System.Net.Http.Json;
using Cliente.Models;
using Microsoft.JSInterop;

namespace Cliente.Services
{
    public class CarritoService
    {
        public int CantidadTotal { get; private set; }

        public event Action? OnChange;

        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public CarritoService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task ActualizarCantidadAsync()
        {
            var carritoId = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");

            if (string.IsNullOrEmpty(carritoId))
            {
                CantidadTotal = 0;
            }
            else
            {
                var items = await _http.GetFromJsonAsync<List<ItemCarrito>>($"api/carritos/{carritoId}") ?? new();
                CantidadTotal = items.Sum(i => i.Cantidad);
            }

            OnChange?.Invoke();
        }

        public async Task AgregarProductoAsync(int productoId, int cantidad)
        {
            var carritoId = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");

            if (string.IsNullOrWhiteSpace(carritoId))
            {
                var response = await _http.PostAsync("api/carritos", null);
                carritoId = await response.Content.ReadAsStringAsync();
                await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", carritoId);
            }

            await _http.PutAsync($"api/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);

            await ActualizarCantidadAsync();
        }
    }
}
