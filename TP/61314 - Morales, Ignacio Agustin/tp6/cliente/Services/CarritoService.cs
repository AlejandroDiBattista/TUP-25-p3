using cliente.shared;

namespace cliente.Services
{
    public class CarritoService
    {
        private readonly List<ItemCarrito> _items = new();
        public IReadOnlyList<ItemCarrito> Items => _items;

        public IReadOnlyList<ItemCarrito> ObtenerItems() => _items;

        public void AgregarProducto(Producto producto)

        {
            var itemExistente = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                _items.Add(new ItemCarrito
                {
                    Producto = producto,
                    Cantidad = 1
                });
            }
        }
         public void CambiarCantidad(int productoId, int cambio)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad += cambio;
                if (item.Cantidad <= 0)
                    _items.Remove(item);
            }
        }

        public void Remover(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
                _items.Remove(item);
        }

        public void Vaciar() => _items.Clear();

        public decimal CalcularTotal() => _items.Sum(i => i.Producto.Precio * i.Cantidad);
    }

    public class ItemCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}
    
