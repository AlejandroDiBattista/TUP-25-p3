// filepath: c:\Users\juare\Documents\UTN - TUP\Programacion 3\tup-25-p3\TP\61129 - Tello, Abril María Agostina\tp6\cliente\Services\CarritoService.cs
using cliente.Modelos;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CarritoService
    {
        public List<CarritoItem> Items { get; set; } = new();

        public void AgregarAlCarrito(Producto producto)
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
        }

        public void QuitarDelCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                    Items.Remove(item);
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
        }

        public decimal Total => Items.Sum(i => i.Producto.Precio * i.Cantidad);
    }
}