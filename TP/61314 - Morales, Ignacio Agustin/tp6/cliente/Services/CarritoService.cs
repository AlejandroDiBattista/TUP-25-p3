using cliente.shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly List<ItemCarrito> _items = new();
        public IReadOnlyList<ItemCarrito> Items => _items;

        public IReadOnlyList<ItemCarrito> ObtenerItems() => _items;

        public event Action<int, int>? CarritoActualizado;
        public Func<int, int>? ObtenerStockDisponible { get; set; }

        private int StockReal(int productoId)
        {
            return ObtenerStockDisponible?.Invoke(productoId) ?? 0;
        }

        public bool AgregarProducto(Producto producto)
        {
            if (producto.Stock <= 0)
                return false;

            var itemExistente = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
           
                var copiaProducto = new Producto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    ImagenUrl = producto.ImagenUrl,
                    Descripcion = producto.Descripcion,
                    Stock = producto.Stock - 1
                };

                _items.Add(new ItemCarrito
                {
                    Producto = copiaProducto,
                    Cantidad = 1
                });
            }

            CarritoActualizado?.Invoke(producto.Id, -1);
            return true;
        }

        public void CambiarCantidad(int productoId, int cambio)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item == null) return;

            int nuevaCantidad = item.Cantidad + cambio;

            if (cambio > 0)
            {
                int stockDisponible = StockReal(productoId);
                if (stockDisponible >= cambio)
                {
                    item.Cantidad += cambio;
                    CarritoActualizado?.Invoke(productoId, -cambio);
                }
            }
            else if (cambio < 0)
            {
                item.Cantidad = nuevaCantidad;
                CarritoActualizado?.Invoke(productoId, Math.Abs(cambio));

                if (item.Cantidad <= 0)
                {
                    _items.Remove(item);
                }
            }
        }

        public void Remover(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                CarritoActualizado?.Invoke(productoId, item.Cantidad);
                _items.Remove(item);
            }
        }

        public void Vaciar()
        {
            foreach (var item in _items.ToList())
            {
                CarritoActualizado?.Invoke(item.Producto.Id, item.Cantidad);
            }
            _items.Clear();
        }

        public decimal CalcularTotal() =>
            _items.Sum(i => i.Producto.Precio * i.Cantidad);

        public void EliminarProducto(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                CarritoActualizado?.Invoke(productoId, item.Cantidad);
                _items.Remove(item);
            }
        }
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
