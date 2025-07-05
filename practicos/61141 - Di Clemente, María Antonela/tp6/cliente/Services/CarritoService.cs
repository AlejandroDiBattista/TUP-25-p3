using cliente.Models;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CarritoService
    {
        public List<ItemCarrito> Items { get; private set; } = new();

        public List<Producto> ProductosDisponibles { get; private set; } = new();

        public void SetProductosDisponibles(List<Producto> productos)
        {
            ProductosDisponibles = productos;
        }

        // Aquí calculamos el stock disponible restando lo que ya está en el carrito
        public int ObtenerStockReal(int productoId)
        {
            var producto = ProductosDisponibles.FirstOrDefault(p => p.Id == productoId);
            var itemEnCarrito = Items.FirstOrDefault(i => i.Producto.Id == productoId);

            if (producto == null) return 0;

            int cantidadEnCarrito = itemEnCarrito?.Cantidad ?? 0;
            int stockReal = producto.Stock - cantidadEnCarrito;
            return stockReal < 0 ? 0 : stockReal;
        }

        public void AgregarProducto(Producto producto, int cantidad)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            int stockReal = ObtenerStockReal(producto.Id);

            if (stockReal == 0) return; // No se puede agregar más

            if (item != null)
            {
                int nuevaCantidad = item.Cantidad + cantidad;
                item.Cantidad = nuevaCantidad <= producto.Stock ? nuevaCantidad : producto.Stock;
            }
            else
            {
                int cantidadAgregar = cantidad <= stockReal ? cantidad : stockReal;
                Items.Add(new ItemCarrito { Producto = producto, Cantidad = cantidadAgregar });
            }
        }

        public void QuitarProducto(Producto producto)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void Vaciar() => Items.Clear();

        public int CantidadTotal() => Items.Sum(i => i.Cantidad);

        public decimal Total() => Items.Sum(i => i.Cantidad * i.Producto.Precio);

        public int TotalItems => CantidadTotal();
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}
