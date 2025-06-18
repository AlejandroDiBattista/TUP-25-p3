#nullable enable
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

        public int CantidadItems { get; private set; }
        public event Action? OnChange;

        public CarritoStateService(IJSRuntime jsRuntime, ApiService apiService)
        {
            _jsRuntime = jsRuntime;
            _apiService = apiService;
        }

        public async Task InicializarAsync()
        {
            _carritoId = await GetOrCreateCarritoIdAsyncInternal();
            await ActualizarCantidadItems();
        }

        private async Task<Guid?> GetOrCreateCarritoIdAsyncInternal()
        {
            if (_carritoId.HasValue)
            {
                return _carritoId.Value;
            }

            var carritoIdString = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", CarritoIdStorageKey);

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

        public async Task<Guid> GetCarritoIdAsync()
        {
            if (!_carritoId.HasValue)
            {
                _carritoId = await GetOrCreateCarritoIdAsyncInternal();
                if (!_carritoId.HasValue) // Still null after trying to create/get
                {
                     throw new Exception("No se pudo obtener el ID del carrito después de la inicialización.");
                }
            }
            return _carritoId.Value;
        }


        public async Task ResetCarritoIdAsync()
        {
            _carritoId = null;
            CantidadItems = 0;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", CarritoIdStorageKey);
            NotifyStateChanged();
        }

        public async Task ActualizarCantidadItems()
        {
            if (!_carritoId.HasValue)
            {
                await InicializarAsync(); 
            }

            if (_carritoId.HasValue)
            {
                var carrito = await _apiService.GetCarritoAsync(_carritoId.Value);
                CantidadItems = carrito?.Sum(i => i.Cantidad) ?? 0;
            }
            else
            {
                CantidadItems = 0;
            }
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
