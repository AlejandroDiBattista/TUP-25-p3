using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        private List<CarritoItem> _items = new();

        public IReadOnlyList<CarritoItem> Items => _items;

        public void AgregarAlCarrito(Producto producto, int cantidad)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
                item.Cantidad += cantidad;
            else
                _items.Add(new CarritoItem { Producto = producto, Cantidad = cantidad });
        }

        public void QuitarDelCarrito(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
                _items.Remove(item);
        }

        public void VaciarCarrito()
        {
            _items.Clear();
        }
    }
}