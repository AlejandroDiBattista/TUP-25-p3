using System;
using System.Collections.Generic;
using System.Linq;
using Cliente.Models2;

namespace Cliente.Services
{
    public class CartService
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public event Action? OnChange;

        public void AddToCart(Producto producto)
        {
            if (producto == null) return;

            var existingItem = _items.FirstOrDefault(ci => ci.Producto.Id == producto.Id);
            if (existingItem != null)
            {
                if (existingItem.Cantidad < producto.Stock)
                    existingItem.Cantidad++;
            }
            else
            {
                _items.Add(new CartItem { Producto = producto, Cantidad = 1 });
            }

            OnChange?.Invoke();
        }

        public void RemoveFromCart(int productoId)
        {
            var existingItem = _items.FirstOrDefault(ci => ci.Producto.Id == productoId);
            if (existingItem != null)
            {
                existingItem.Cantidad--;
                if (existingItem.Cantidad <= 0)
                    _items.Remove(existingItem);

                OnChange?.Invoke();
            }
        }

        public void IncrementarCantidad(int productoId)
        {
            var item = _items.FirstOrDefault(ci => ci.Producto.Id == productoId);
            if (item != null && item.Cantidad < item.Producto.Stock)
            {
                item.Cantidad++;
                OnChange?.Invoke();
            }
        }

        public void DecrementarCantidad(int productoId)
        {
            var item = _items.FirstOrDefault(ci => ci.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                    _items.Remove(item);

                OnChange?.Invoke();
            }
        }

        public void ClearCart()
        {
            _items.Clear();
            OnChange?.Invoke();
        }

        public decimal GetTotal() =>
            _items.Sum(ci => ci.Producto.Precio * ci.Cantidad);

        public int GetCantidadTotal() =>
            _items.Sum(ci => ci.Cantidad);
    }

    public class CartItem
    {
        public Producto Producto { get; set; } = new();
        public int Cantidad { get; set; } = 1;
    }
}
