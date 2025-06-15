using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TuProyecto.Models;
using System.Text.Json;


namespace cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient httpClient;
        public int? CarritoId { get; set; }
        public Compra? CarritoActual { get; set; }

        public event Action? OnChange;

        public CarritoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Crear un nuevo carrito (si no existe)
        public async Task CrearCarritoSiNoExisteAsync()
        {
            if (CarritoId != null && CarritoId != 0) return;
            var response = await httpClient.PostAsync("/api/carrito", null);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                CarritoId = json.GetProperty("carritoId").GetInt32();
            }
        }

        // Agregar o actualizar cantidad de un producto
        public async Task AgregarOActualizarProductoAsync(int productoId, int cantidad)
        {
            await CrearCarritoSiNoExisteAsync();
            var response = await httpClient.PutAsync($"/api/carrito/{CarritoId}/{productoId}?cantidad={cantidad}", null);
            if (response.IsSuccessStatusCode)
                await RefrescarCarritoAsync();
        }

        // Refrescar el carrito desde el backend
        public async Task RefrescarCarritoAsync()
        {
            if (CarritoId == null || CarritoId == 0) return;
            CarritoActual = await httpClient.GetFromJsonAsync<Compra>($"/api/carrito/{CarritoId}");
            OnChange?.Invoke();
        }

        // Eliminar un producto del carrito
        public async Task EliminarProductoAsync(int productoId)
        {
            if (CarritoId == null) return;
            await httpClient.DeleteAsync($"/api/carrito/{CarritoId}/{productoId}");
            await RefrescarCarritoAsync();
        }

        // Vaciar el carrito
        public async Task VaciarCarritoAsync()
        {
            if (CarritoId == null) return;
            await httpClient.DeleteAsync($"/api/carrito/{CarritoId}");
            await RefrescarCarritoAsync();
        }

        // Confirmar la compra
        public async Task ConfirmarCompraAsync(string nombre, string apellido, string email)
        {
            if (CarritoId == null) return;
            var datos = new Compra
            {
                NombreCliente = nombre,
                ApellidoCliente = apellido,
                EmailCliente = email
            };
            await httpClient.PutAsJsonAsync($"/api/carrito/{CarritoId}/confirmar", datos);
            await RefrescarCarritoAsync();
        }
    }
}