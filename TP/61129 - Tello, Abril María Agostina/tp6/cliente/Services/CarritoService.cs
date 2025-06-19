using cliente.Modelos;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly IJSRuntime js;
        public List<CarritoItem> Items { get; set; } = new();

        public CarritoService(IJSRuntime js)
        {
            this.js = js;
            _ = CargarCarritoAsync();
        }

        public async Task AgregarAlCarrito(Producto producto)
        {
            if (producto.Stock > 0)
            {
                var item = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
                if (item != null)
                {
                    item.Cantidad++;
                }
                else
                {
                    Items.Add(new CarritoItem { ProductoId = producto.Id, Cantidad = 1 });
                }
                producto.Stock--;
                await GuardarCarritoAsync();
            }
        }

        public async Task QuitarDelCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
            if (item != null)
            {
                item.Cantidad--;
                producto.Stock++;
                if (item.Cantidad <= 0)
                    Items.Remove(item);
            }
            await GuardarCarritoAsync();
        }

        public async Task VaciarCarrito()
        {
            Items.Clear();
            await GuardarCarritoAsync();
        }

        public async Task GuardarCarritoAsync()
        {
            await js.InvokeVoidAsync("localStorage.setItem", "carrito", JsonSerializer.Serialize(Items));
        }

        private async Task CargarCarritoAsync()
        {
            var json = await js.InvokeAsync<string>("localStorage.getItem", "carrito");
            if (!string.IsNullOrEmpty(json))
                Items = JsonSerializer.Deserialize<List<CarritoItem>>(json);
        }

        public decimal CalcularTotal(List<Producto> productos)
        {
            return Items.Sum(i =>
            {
                var producto = productos.FirstOrDefault(p => p.Id == i.ProductoId);
                return producto != null ? producto.Precio * i.Cantidad : 0;
            });
        }

        public async Task EliminarProductoDelCarrito(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                Items.Remove(item);
                await GuardarCarritoAsync();
            }
        }
    }
}