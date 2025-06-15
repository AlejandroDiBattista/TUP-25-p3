using Cliente.Modelos;

namespace Cliente.Services
{
    public class CarritoService
    {
        private List<CarritoItem> _items = new();

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
        public List<CarritoItem> ObtenerItems() => _items;
        public void Limpiar() => _items.Clear();

        public decimal CalcularTotal() => _items.Sum(i => i.Producto.Precio * i.Cantidad);
    }
}