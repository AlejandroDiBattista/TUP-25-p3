    namespace cliente.Services;
    
    public class EstadoCarrito {
        public int? CarritoId { get; private set; }
        public int ItemsEnCarrito { get; private set; }
        public event Action? OnChange;
        public void SetCarritoId(int id) => CarritoId = id;
        public void SetItemsEnCarrito(int conteo) {
            if (ItemsEnCarrito != conteo) {
                ItemsEnCarrito = conteo;
                NotifyStateChanged();
            }
        }
        public void LimpiarCarrito() {
            CarritoId = null;
            ItemsEnCarrito = 0;
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
    