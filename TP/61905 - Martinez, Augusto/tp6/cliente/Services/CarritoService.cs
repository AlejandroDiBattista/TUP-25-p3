using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Cliente.Models;

namespace Cliente.Services
{
    public class CarritoService
    {
        private readonly HttpClient _httpClient;

        public CarritoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<CarritoItem> Items { get; private set; } = new();

        public decimal Total => Items.Sum(i => i.Importe);

        // ✅ Cargar manualmente items desde otro componente
        public void CargarItems(List<CarritoItem> nuevosItems)
        {
            Items = nuevosItems ?? new List<CarritoItem>();
        }

        // ✅ Obtener carrito completo por usuario
        public async Task<Carrito> ObtenerCarritoAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/carrito/{usuarioId}");
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error {response.StatusCode}");

                return await response.Content.ReadFromJsonAsync<Carrito>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener el carrito: {ex.Message}");
                return null;
            }
        }

        // ✅ Usado al iniciar la página de Confirmar.razor
        public async Task CargarDesdeApiAsync(int usuarioId)
        {
            var carrito = await ObtenerCarritoAsync(usuarioId);
            if (carrito != null)
                CargarItems(carrito.CarritoItems);
        }

        // ✅ Agregar un producto al carrito
        public async Task<HttpResponseMessage> AgregarProductoAsync(Guid carritoId, int productoId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/carrito/{carritoId}/{productoId}", null);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error {response.StatusCode}");

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al agregar producto: {ex.Message}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        // ✅ Total de unidades en el carrito (forma rápida)
        public async Task<int> ObtenerCantidadProductos(Guid carritoId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<int>($"api/carrito/contador/{carritoId}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener cantidad de productos: {ex.Message}");
                return 0;
            }
        }

        // ✅ Alternativa con control de estado HTTP
        public async Task<int> ObtenerContadorProductosAsync(Guid carritoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/carrito/contador/{carritoId}");
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error {response.StatusCode}");

                return await response.Content.ReadFromJsonAsync<int>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener el contador de productos: {ex.Message}");
                return 0;
            }
        }

        // ✅ Eliminar un producto del carrito
        public async Task EliminarProductoAsync(int productoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/carrito/eliminar/{productoId}");
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error {response.StatusCode}");

                Items.RemoveAll(i => i.ProductoId == productoId);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
            }
        }

        // ✅ Vaciar el carrito por usuario
        public async Task VaciarCarritoAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/carrito/{usuarioId}");
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error {response.StatusCode}");

                Items.Clear();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al vaciar el carrito: {ex.Message}");
            }
        }

        // ✅ Confirmar compra y enviar datos al backend
public async Task ConfirmarCompraAsync(int usuarioId, string nombre, string email)
{
    try
    {
        var venta = new Venta
        {
            UsuarioId = usuarioId,
            Fecha = DateTime.UtcNow,
            Total = Items.Sum(i => i.Importe),

            NombreCliente = nombre,
            ApellidoCliente = "-", // o agregá campo en el formulario
            EmailCliente = email,

            VentaItems = Items.Select(item => new VentaItem
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario
            }).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("api/ventas/confirmar", venta);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Error {response.StatusCode}");

        await VaciarCarritoAsync(usuarioId);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Error al confirmar la compra: {ex.Message}");
    }
}


        // ✅ Actualizar precios o stock en UI luego de una compra
        public void ActualizarProductos(List<Producto> productosActualizados)
        {
            foreach (var actualizado in productosActualizados)
            {
                var item = Items.FirstOrDefault(i => i.ProductoId == actualizado.Id);
                if (item != null)
                {
                    item.PrecioUnitario = actualizado.Precio;
                }
            }
        }
    }
}
