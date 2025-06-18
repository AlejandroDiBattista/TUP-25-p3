using System;
using System.Collections.Generic;
using System.Linq;
using Cliente.Models2; // Usa tu clase Producto

namespace Cliente.Services
{
    public class CartService
    {
        public List<Producto> Items { get; } = new();

        public event Action? OnChange;

        public void AddToCart(Producto producto)
        {
            Items.Add(producto);
            OnChange?.Invoke();
        }

        public void RemoveFromCart(int productoId)
        {
            var item = Items.FirstOrDefault(p => p.Id == productoId);
            if (item != null)
            {
                Items.Remove(item);
                OnChange?.Invoke();
            }
        }

        public void ClearCart()
        {
            Items.Clear();
            OnChange?.Invoke();
        }
    }
}
