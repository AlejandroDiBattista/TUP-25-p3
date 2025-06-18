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
        private const string Key = "productos_stock";

        public StockLocalService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task GuardarStockAsync(List<Producto> productos)
        {
            var json = JsonSerializer.Serialize(productos);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", Key, json);
        }

        public async Task<List<Producto>> LeerStockAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", Key);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<List<Producto>>(json);
        }
    }
}
