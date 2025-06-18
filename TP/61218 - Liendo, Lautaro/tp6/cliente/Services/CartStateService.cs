// Services/CartStateService.cs
using cliente.Models;
using System.Collections.Generic;

namespace cliente.Services
{
    // Servicio para gestionar el estado global del carrito y notificar cambios a los componentes.
    public class CartStateService
    {
        // Evento que los componentes pueden suscribirse para ser notificados de cambios en el carrito.
        public event Action? OnChange;

        // ID del carrito actual. Si es nulo, no hay carrito activo.
        private string? _cartId;
        public string? CartId
        {
            get => _cartId;
            private set
            {
                if (_cartId != value)
                {
                    _cartId = value;
                    NotifyStateChanged(); // Notifica a los suscriptores cuando el ID cambia
                }
            }
        }

        // Lista de ítems en el carrito.
        private List<DetalleCarritoMemoria> _cartItems = new List<DetalleCarritoMemoria>();
        public IReadOnlyList<DetalleCarritoMemoria> CartItems => _cartItems.AsReadOnly();

        // Propiedad calculada para obtener el número total de ítems (unidades) en el carrito.
        public int TotalItemsInCart => _cartItems.Sum(item => item.Unidades);

        // Establece el ID del carrito y notifica el cambio.
        public void SetCartId(string? newCartId)
        {
            CartId = newCartId;
        }

        // Establece la lista de ítems del carrito y notifica el cambio.
        public void SetCartItems(List<DetalleCarritoMemoria>? newItems)
        {
            _cartItems = newItems ?? new List<DetalleCarritoMemoria>();
            NotifyStateChanged(); // Notifica a los suscriptores cuando los ítems cambian
        }

        // Vacía el carrito completamente (ID e ítems).
        public void ClearCart()
        {
            CartId = null;
            _cartItems = new List<DetalleCarritoMemoria>();
            NotifyStateChanged();
        }

        // Método para invocar el evento OnChange.
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
