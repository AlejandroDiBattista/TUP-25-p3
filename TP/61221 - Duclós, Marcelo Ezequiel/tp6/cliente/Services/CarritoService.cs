using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        private List<CarritoItem> _items = new();

        public IReadOnlyList<CarritoItem> Items => _items;

        // Evento para notificar cambios
        public event Action OnChange;

        public void AgregarAlCarrito(Producto producto, int cantidad)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
                item.Cantidad += cantidad;
            else
                _items.Add(new CarritoItem { Producto = producto, Cantidad = cantidad });
            
            NotificarCambio();
        }

        public void QuitarDelCarrito(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                _items.Remove(item);
                NotificarCambio();
            }
        }

        public void VaciarCarrito()
        {
            _items.Clear();
            NotificarCambio();
        }

        public bool ModificarCantidad(int productoId, int nuevaCantidad)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                // Validar que no se exceda el stock original del producto
                if (nuevaCantidad > item.Producto.Stock)
                {
                    // No hay suficiente stock
                    return false;
                }
                
                item.Cantidad = nuevaCantidad;
                
                if (item.Cantidad <= 0)
                {
                    _items.Remove(item);
                }
                
                NotificarCambio();
                return true;
            }
            return false;
        }

        // Método helper para obtener el stock disponible de un producto
        public int ObtenerStockDisponible(Producto producto)
        {
            var cantidadEnCarrito = _items
                .Where(i => i.Producto.Id == producto.Id)
                .Sum(i => i.Cantidad);
            
            return Math.Max(0, producto.Stock - cantidadEnCarrito);
        }

        private void NotificarCambio()
        {
            OnChange?.Invoke();
        }
    }
}