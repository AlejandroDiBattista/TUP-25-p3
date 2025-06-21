using Cliente.Modelos;

namespace Cliente.Services
{
    public class CarritoService
    {
        private List<CarritoItem> _items = new();
        public event Action<int, int>? OnProductoRemovido;
        public void AgregarProducto(Producto producto)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                _items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });
            }
        }
        public void SumarCantidad(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
                item.Cantidad++;
        }

        public void RestarCantidad(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                {
                    _items.Remove(item);
                    OnProductoRemovido?.Invoke(productoId, 1);
                }
                else
                {
                    OnProductoRemovido?.Invoke(productoId, 1);
                }
            }
        }

        public void EliminarProducto(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                int cantidadDevuelta = item.Cantidad;
                _items.Remove(item);
                OnProductoRemovido?.Invoke(productoId, cantidadDevuelta);
            }
        }
        public List<CarritoItem> ObtenerItems() => _items;
        public void Limpiar() => _items.Clear();

        public decimal CalcularTotal() => _items.Sum(i => i.Producto.Precio * i.Cantidad);
    }
}