using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        private int? _carritoId;

        public CarritoService(HttpClient http)
        {
            _http = http;
        }

        public int? CarritoId => _carritoId;

        // Inicializa un carrito nuevo si no hay uno activo
        public async Task<int> ObtenerOCrearCarritoAsync()
        {
            if (_carritoId.HasValue)
                return _carritoId.Value;

            var response = await _http.PostAsync("/api/carritos", null);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<CrearCarritoResponse>();
                _carritoId = data!.CarritoId;
                return _carritoId.Value;
            }

            throw new Exception("No se pudo crear el carrito.");
        }

        // Agrega producto al carrito
        public async Task<bool> AgregarProductoAsync(int productoId)
        {
            var carritoId = await ObtenerOCrearCarritoAsync();
            var response = await _http.PutAsync($"/api/carritos/{carritoId}/{productoId}", null);
            return response.IsSuccessStatusCode;
        }

        // Obtiene los ítems actuales del carrito
        public async Task<List<CarritoItem>> ObtenerItemsAsync()
        {
            var carritoId = await ObtenerOCrearCarritoAsync();
            var items = await _http.GetFromJsonAsync<List<CarritoItem>>($"/api/carritos/{carritoId}");
            return items ?? new List<CarritoItem>();
        }

        private class CrearCarritoResponse
        {
            public int CarritoId { get; set; }
        }

        // Quitar una unidad (DELETE /api/carritos/{carritoId}/{productoId})
        public async Task<bool> QuitarUnidadAsync(int productoId)
        {
            var carritoId = await ObtenerOCrearCarritoAsync();
            var response = await _http.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
            return response.IsSuccessStatusCode;
        }

        // Quitar todo el producto del carrito (ejecuta DELETE varias veces)
        public async Task<bool> QuitarTodoAsync(int productoId)
        {
            var ok = true;
            // Opcional: podrías pedir el item y saber cuántas unidades hay,
            // pero más simple: hacé un loop con protección anti-infinito
            for (int i = 0; i < 20; i++)
            {
                var result = await QuitarUnidadAsync(productoId);
                if (!result) break;
            }
            return ok;
        }

        // Vaciar todo el carrito (DELETE /api/carritos/{carritoId})
        public async Task<bool> VaciarCarritoAsync()
        {
            var carritoId = await ObtenerOCrearCarritoAsync();
            var response = await _http.DeleteAsync($"/api/carritos/{carritoId}");
            return response.IsSuccessStatusCode;
        }

        // Confirmar la compra (PUT /api/carritos/{carritoId}/confirmar)
        public async Task<bool> ConfirmarCompraAsync(object form)
        {
            var carritoId = await ObtenerOCrearCarritoAsync();
            var response = await _http.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", form);
            return response.IsSuccessStatusCode;
        }
    }
}


