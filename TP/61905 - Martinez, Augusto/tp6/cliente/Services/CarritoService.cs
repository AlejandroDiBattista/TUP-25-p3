using Cliente.Models;
using System.Collections.Generic;
using System.Linq;

namespace Cliente.Services
{
    public class CarritoService
    {
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
    }
}

