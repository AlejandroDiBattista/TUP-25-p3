using cliente.Models;
using System.Collections.Generic;
using System.Linq;
using cliente.Models;


namespace cliente.Services
{
    public class CarritoService
    {
        public List<ItemCarrito> Items { get; set; } = new();

        public decimal Total => Items.Sum(i => i.Cantidad * i.Producto.Precio);

        public int CantidadTotal => Items.Sum(i => i.Cantidad);

        
    public void VaciarCarrito()
    {
        Items.Clear();
    }

        public void AgregarAlCarrito(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
            }
        }
    }
}
