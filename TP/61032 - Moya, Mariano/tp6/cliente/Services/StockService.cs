using System;

namespace Cliente.Services
{
    public class StockService
    {
        public event Action? OnStockChanged;

        public void NotificarCambioStock()
        {
            OnStockChanged?.Invoke();
        }
    }
}
