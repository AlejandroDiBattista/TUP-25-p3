using System.Text.Json;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using cliente.Models;

namespace cliente.Services
{
    public class StockLocalService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string KeyInicial = "productos_stock_inicial";
        private const string KeyActual = "productos_stock";

        public StockLocalService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task GuardarStockAsync(List<Producto> productos)
        {
            var json = JsonSerializer.Serialize(productos);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", KeyActual, json);
        }

        public async Task<List<Producto>> LeerStockAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", KeyActual);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<List<Producto>>(json);
        }

        public async Task GuardarStockInicialAsync(List<Producto> productos)
        {
            var json = JsonSerializer.Serialize(productos);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", KeyInicial, json);
        }

        public async Task<List<Producto>> LeerStockInicialAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", KeyInicial);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<List<Producto>>(json);
        }

        public async Task RestaurarStockInicialAsync()
        {
            var stockInicial = await LeerStockInicialAsync();
            if (stockInicial != null)
            {
                var json = JsonSerializer.Serialize(stockInicial);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", KeyActual, json);
            }
        }
    }
}
