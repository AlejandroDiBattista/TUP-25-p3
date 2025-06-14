using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace cliente.Services
{
    public class CarritoState
    {
        public event Action OnChange;
        private int _cantidad;
        public int Cantidad
        {
            get => _cantidad;
            set { _cantidad = value; NotifyStateChanged(); }
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
