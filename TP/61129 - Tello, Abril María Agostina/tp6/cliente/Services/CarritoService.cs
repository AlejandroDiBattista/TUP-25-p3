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
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                if (item.Cantidad < producto.Stock)
                    item.Cantidad++;
            }
            else
            {
                if (producto.Stock > 0)
                    Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
            }
            await GuardarCarritoAsync();
        }

        public async Task QuitarDelCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad--;
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

        private async Task GuardarCarritoAsync()
        {
            await js.InvokeVoidAsync("localStorage.setItem", "carrito", JsonSerializer.Serialize(Items));
        }

        private async Task CargarCarritoAsync()
        {
            var json = await js.InvokeAsync<string>("localStorage.getItem", "carrito");
            if (!string.IsNullOrEmpty(json))
                Items = JsonSerializer.Deserialize<List<CarritoItem>>(json);
        }

        public decimal Total => Items.Sum(i => i.Producto.Precio * i.Cantidad);
    }
}