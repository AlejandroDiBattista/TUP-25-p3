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
    }
}
