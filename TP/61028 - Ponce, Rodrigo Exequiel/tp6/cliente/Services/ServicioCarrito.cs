using cliente.Modelos;
using System.Collections.Generic;   
using System.Linq;

namespace cliente.Services
{
    public class ServicioCarrito
    {
        public List<ItemCarrito> Items { get; set; } = new();

        public void AgregarAlCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
                item.Cantidad++;
            else
                Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
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
    }
}