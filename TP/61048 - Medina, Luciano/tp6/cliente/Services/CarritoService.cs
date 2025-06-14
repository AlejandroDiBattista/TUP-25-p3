using System.Net.Http.Json;
using cliente.Shared;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        private bool isInitializing = false;

        public int? CarritoId { get; private set; }
        public int CantidadItems { get; private set; }
        public event Action? OnChange;

        public CarritoService(HttpClient http)
        {
            _http = http;
        }

        // --- NUEVO MÉTODO CENTRALIZADO ---
        public async Task<bool> AgregarProductoAlCarrito(int productoId)
        {
            await InicializarCarrito();
            if (CarritoId.HasValue)
            {
                var response = await _http.PutAsync($"/api/carritos/{CarritoId}/agregar/{productoId}?cantidad=1", null);
                if (response.IsSuccessStatusCode)
                {
                    await ActualizarCantidadDesdeApi();
                    return true;
                }
            }
            return false;
        }

        public async Task InicializarCarrito()
        {
            if (CarritoId != null || isInitializing) return;

            try
            {
                isInitializing = true;
                var response = await _http.PostAsync("/api/carritos", null);
                if (response.IsSuccessStatusCode)
                {
                    CarritoId = await response.Content.ReadFromJsonAsync<int>();
                }
            }
            finally
            {
                isInitializing = false;
            }
        }

        public async Task ActualizarCantidadDesdeApi()
        {
            if (!CarritoId.HasValue) return;

            try
            {
                var compraDto = await _http.GetFromJsonAsync<CompraResumenDto>($"/api/carritos/{CarritoId.Value}");
                this.CantidadItems = compraDto?.Items.Sum(i => i.Cantidad) ?? 0;
                NotifyStateChanged(); 
            }
            catch
            {
                this.CantidadItems = 0;
                NotifyStateChanged();
            }
        }
        
        public void EstablecerCantidad(int cantidad)
        {
            if (CantidadItems != cantidad)
            {
                CantidadItems = cantidad;
                NotifyStateChanged();
            }
        }
        
        public async Task LimpiarCarrito()
        {
            CarritoId = null;
            await InicializarCarrito();
            await ActualizarCantidadDesdeApi();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
