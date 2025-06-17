using System;

namespace Cliente.Services
{
    public class BusquedaService
    {
        private string _texto = string.Empty;
        public string TextoBusqueda
        {
            get => _texto;
            set
            {
                if (_texto != value)
                {
                    _texto = value;
                    OnBusquedaChanged?.Invoke(_texto);
                }
            }
        }
        public event Action<string>? OnBusquedaChanged;
    }
}
