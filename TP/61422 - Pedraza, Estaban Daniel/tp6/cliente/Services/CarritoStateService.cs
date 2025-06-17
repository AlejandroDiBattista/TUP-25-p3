using Blazored.LocalStorage;

namespace cliente.Services
{
    public class CarritoStateService
    {
        private readonly ILocalStorageService _localStorage;
        private const string CarritoIdKey = "carritoId";
        public event Action OnCarritoChanged = delegate { };

        public CarritoStateService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<string> GetCarritoIdAsync()
        {
            return await _localStorage.GetItemAsync<string>(CarritoIdKey);
        }

        public async Task SetCarritoIdAsync(string carritoId)
        {
            await _localStorage.SetItemAsync(CarritoIdKey, carritoId);
        }

        public void NotificarCambio()
        {
            OnCarritoChanged?.Invoke();
        }
    }
}