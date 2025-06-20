using System;
using System.Collections.Generic;
using System.Linq;
using Cliente.Models2;

namespace Cliente.Services
{
    public class CartService
    {
        private readonly List<CartItem> _items = new();

        /// <summary>
        /// Lista pública de productos en el carrito (solo lectura).
        /// </summary>
        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        /// <summary>
        /// Evento que notifica cambios al carrito.
        /// </summary>
        public event Action? OnChange;

        public void AddToCart(Producto producto)
        {
            if (producto == null) return;

            var item = _items.FirstOrDefault(ci => ci.Producto.Id == producto.Id);

            if (item != null)
            {
                if (item.Cantidad < producto.Stock)
                {
                    item.Cantidad++;
                    OnChange?.Invoke();
                }
            }
            else
            {
                _items.Add(new CartItem { Producto = producto, Cantidad = 1 });
                OnChange?.Invoke();
            }
        }

        public void RemoveFromCart(int productoId)
        {
            var item = _items.FirstOrDefault(ci => ci.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                {
                    _items.Remove(item);
                }

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
            if (_items.Any())
            {
                _items.Clear();
                OnChange?.Invoke();
            }
        }

        public decimal GetTotal() =>
            _items.Sum(ci => ci.Producto.Precio * ci.Cantidad);

        public int GetCantidadTotal() =>
            _items.Sum(ci => ci.Cantidad);
    }

    /// <summary>
    /// Representa un ítem dentro del carrito.
    /// </summary>
    public class CartItem
    {
        public Producto Producto { get; set; } = new();
        public int Cantidad { get; set; } = 1;
    }
}
