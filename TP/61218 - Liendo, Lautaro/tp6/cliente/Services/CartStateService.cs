using cliente.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CartStateService
    {
        public event Action? OnChange;

        private string? _cartId;
        public string? CartId
        {
            get => _cartId;
            private set
            {
                if (_cartId != value)
                {
                    _cartId = value;
                    NotifyStateChanged();
                }
            }
        }

        private List<DetalleCarritoMemoria> _cartItems = new List<DetalleCarritoMemoria>();
        public IReadOnlyList<DetalleCarritoMemoria> CartItems => _cartItems.AsReadOnly();

        public int TotalItemsInCart => _cartItems.Sum(item => item.Unidades);

        public void SetCartId(string? newCartId)
        {
            CartId = newCartId;
        }

        public void SetCartItems(List<DetalleCarritoMemoria> items)
        {
            _cartItems = items;
            NotifyStateChanged();
        }

        public void ClearCart()
        {
            CartId = null;
            _cartItems = new List<DetalleCarritoMemoria>();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
