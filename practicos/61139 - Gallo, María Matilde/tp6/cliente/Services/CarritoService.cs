using cliente.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly List<CarritoItem> items = new();

        public event Action OnChange;

        public IReadOnlyList<CarritoItem> Items => items.AsReadOnly();

        public int TotalItems => items.Sum(i => i.Cantidad);

        public decimal TotalPrecio => items.Sum(i => i.Producto.Precio * i.Cantidad);

        public void AgregarProducto(Producto producto)
        {
            if (producto.Stock > 0) // Solo si hay stock disponible
            {
                var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
                if (item == null)
                {
                    items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
                }
                else
                {
                    item.Cantidad++;
                }
                producto.Stock--; // Descuenta 1 del stock disponible
                NotifyStateChanged();
            }
        }


        public void SumarCantidad(int productoId)
        {
            var item = items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad++;
                NotifyStateChanged();
            }
        }

        public void RestarCantidad(int productoId)
        {
            var item = items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                    items.Remove(item);
                NotifyStateChanged();
            }
        }

        public void Vaciar()
        {
            items.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
        
    }

    public class CarritoItem
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}