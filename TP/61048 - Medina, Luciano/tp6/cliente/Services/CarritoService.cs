using System.Net.Http.Json;
using cliente.Shared; // Usaremos los modelos compartidos

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _http;
        public int? CarritoId { get; private set; }
        public int CantidadItems { get; private set; }
        
        // Evento que se dispara cuando el carrito cambia. 
        // El layout se suscribirá a esto para actualizar el contador.
        public event Action? OnChange;

        public CarritoService(HttpClient http)
        {
            _http = http;
        }

        public async Task InicializarCarrito()
        {
            if (CarritoId == null)
            {
                // Llama a la API para crear un nuevo carrito (compra en estado pendiente)
                var response = await _http.PostAsync("/api/carritos", null);
                if (response.IsSuccessStatusCode)
                {
                    CarritoId = await response.Content.ReadFromJsonAsync<int>();
                    await ActualizarCantidad();
                }
            }
        }

        public async Task ActualizarCantidad()
        {
            if (!CarritoId.HasValue) return;

            try
            {
                var compra = await _http.GetFromJsonAsync<Compra>($"/api/carritos/{CarritoId.Value}");
                CantidadItems = compra?.Items.Sum(i => i.Cantidad) ?? 0;
            }
            catch
            {
                CantidadItems = 0; // Si el carrito no se encuentra o hay error, la cantidad es 0
            }
            
            // Notificar a todos los suscriptores (como el MainLayout) que hubo un cambio.
            NotifyStateChanged();
        }

        public async Task LimpiarCarrito()
        {
            CarritoId = null;
            CantidadItems = 0;
            await InicializarCarrito();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}