namespace cliente.Services
{
    public class BusquedaService
    {
        private string _busqueda;
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                if (_busqueda != value)
                {
                    _busqueda = value;
                    OnBusquedaChanged?.Invoke();
                }
            }
        }

        public event Action OnBusquedaChanged;
    }
}