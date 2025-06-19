using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using cliente.Models;

namespace cliente.Services
{
    public class CartState : IDisposable
    {
        private int _cartId;
        public int CartId   => _cartId;
        public int ItemCount { get; private set; }

        public void SetCartId(int id)
        {
            _cartId = id;
            NotifyStateChanged();
        }

        public async Task RefreshCountAsync(HttpClient http)
        {
            if (_cartId == 0)
            {
                ItemCount = 0;
            }
            else
            {
                var items = await http.GetFromJsonAsync<List<ItemCompra>>($"/carritos/{_cartId}");
                ItemCount = items?.Sum(i => i.Cantidad) ?? 0;
            }
            NotifyStateChanged();
        }

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public void Dispose()
        {
            OnChange = null;
        }
    }
}