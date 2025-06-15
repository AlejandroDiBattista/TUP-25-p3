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

        public void Agregar(CarritoItem nuevoItem)
        {
            var existente = Items.FirstOrDefault(i => i.ProductoId == nuevoItem.ProductoId);
            if (existente != null)
            {
                existente.Cantidad += nuevoItem.Cantidad;
            }
            else
            {
                Items.Add(nuevoItem);
            }
        }

        public void ModificarCantidad(int productoId, int cambio)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item == null) return;

            item.Cantidad += cambio;
            if (item.Cantidad <= 0)
            {
                Items.Remove(item);
            }
        }

        public void Vaciar()
        {
            Items.Clear();
        }

        public void AgregarProducto(Producto producto)
        {
            var existente = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
            if (existente != null)
            {
                existente.Cantidad++;
            }
            else
            {
                Items.Add(new CarritoItem
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    PrecioUnitario = producto.Precio,
                    Cantidad = 1
                });
            }

            producto.Stock--; // Opcional
        }

        public int ContadorProductos()
        {
            return Items.Sum(i => i.Cantidad);
        }

        // 🔥 Nuevo método para actualizar productos después de la compra
        public void ActualizarProductos(List<Producto> productosActualizados)
        {
            foreach (var actualizado in productosActualizados)
            {
                var itemEnCarrito = Items.FirstOrDefault(i => i.ProductoId == actualizado.Id);
                if (itemEnCarrito != null)
                {
                    itemEnCarrito.PrecioUnitario = actualizado.Precio;
                }
            }
        }

        // ✅ Método mejorado para confirmar compra y reflejar stock actualizado
        public async Task ConfirmarCompraAsync(string clienteNombre, string clienteEmail)
        {
            var venta = new Venta
            {
                ClienteNombre = clienteNombre,
                ClienteEmail = clienteEmail,
                Items = Items.Select(item => new VentaItem
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("api/comprar", venta);

            if (response.IsSuccessStatusCode)
            {
                var productosActualizados = await _httpClient.GetFromJsonAsync<List<Producto>>("api/productos");
                if (productosActualizados != null)
                {
                    ActualizarProductos(productosActualizados);
                }

                Vaciar(); // Vaciar el carrito después de la compra exitosa
            }
            else
            {
                throw new HttpRequestException("Error al confirmar la compra");
            }
        }
    }
}
