using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using Compartido;

namespace cliente.Services
{
    public class CarritoStateService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ApiService _apiService;
        private const string CarritoIdStorageKey = "carritoId";
        private Guid? _carritoId;

        public CarritoStateService(IJSRuntime jsRuntime, ApiService apiService)
        {
            _jsRuntime = jsRuntime;
            _apiService = apiService;
        }

        public async Task<Guid> GetOrCreateCarritoIdAsync()
        {
            if (_carritoId.HasValue)
            {
                return _carritoId.Value;
            }

            var carritoIdString = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", CarritoIdStorageKey);

            if (!string.IsNullOrEmpty(carritoIdString) && Guid.TryParse(carritoIdString, out var storedGuid))
            {
                _carritoId = storedGuid;
                return _carritoId.Value;
            }

            
            var nuevaCompra = await _apiService.CrearCarritoAsync();
            if (nuevaCompra != null)
            {
                _carritoId = nuevaCompra.Id;
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CarritoIdStorageKey, _carritoId.Value.ToString());
                return _carritoId.Value;
            }
            
            throw new Exception("No se pudo crear o recuperar el ID del carrito.");
        }

        public async Task ResetCarritoIdAsync()
        {
            _carritoId = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", CarritoIdStorageKey);
        }
    }
}
