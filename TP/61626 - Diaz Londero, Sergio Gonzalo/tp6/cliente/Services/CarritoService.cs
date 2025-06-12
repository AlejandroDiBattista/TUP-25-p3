using System.Linq;
using System.Collections.Generic;

namespace cliente.Services
{
    public class CarritoService
    {
        public List<ItemCarrito> Items { get; set; } = new();

        public bool AgregarProducto(Producto producto)
        {
            // Validar stock antes de agregar
            int stockDisponible = producto.Stock - CantidadEnCarrito(producto.Id);

            if (stockDisponible <= 0)
                return false; // No hay stock para agregar

            var existente = Items.FirstOrDefault(p => p.Producto.Id == producto.Id);
            if (existente != null)
            {
                existente.Cantidad++;
            }
            else
            {
                Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
            }
            return true;
        }

        public void EliminarProducto(int productoId)
        {
            var item = Items.FirstOrDefault(p => p.Producto.Id == productoId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
        }

        public decimal CalcularTotal()
        {
            return Items.Sum(i => i.Cantidad * i.Producto.Precio);
        }

        private int CantidadEnCarrito(int productoId)
        {
            var item = Items.FirstOrDefault(p => p.Producto.Id == productoId);
            return item?.Cantidad ?? 0;
        }
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}
