using System;
using System.Collections.Generic;
using System.Linq;
using Cliente.Models2;

namespace Cliente.Services
{
    public class CartService
    {
        public List<CartItem> Items { get; } = new();

        public event Action? OnChange;

        public void AddToCart(Producto producto)
        {
            var existingItem = Items.FirstOrDefault(ci => ci.Producto.Id == producto.Id);
            if (existingItem != null)
            {
                existingItem.Cantidad++;
            }
            else
            {
                Items.Add(new CartItem { Producto = producto, Cantidad = 1 });
            }
            OnChange?.Invoke();
        }

        public void RemoveFromCart(int productoId)
        {
            var existingItem = Items.FirstOrDefault(ci => ci.Producto.Id == productoId);
            if (existingItem != null)
            {
                existingItem.Cantidad--;
                if (existingItem.Cantidad <= 0)
                    Items.Remove(existingItem);

                OnChange?.Invoke();
            }
        }

        public void ClearCart()
        {
            Items.Clear();
            OnChange?.Invoke();
        }

        public decimal GetTotal()
        {
            return Items.Sum(ci => ci.Producto.Precio * ci.Cantidad);
        }

        public int GetCantidadTotal()
        {
            return Items.Sum(ci => ci.Cantidad);
        }
    }

    public class CartItem
    {
        public Producto Producto { get; set; } = new Producto();
        public int Cantidad { get; set; } = 1;
    }
}
