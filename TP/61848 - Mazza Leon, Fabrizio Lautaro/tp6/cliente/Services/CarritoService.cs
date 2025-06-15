using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        public int CarritoId { get; set; } = 0;
        public List<ItemCarrito> items = new();

        public IReadOnlyList<ItemCarrito> Items => items;
    
        public int CantidadTotal => Items.Sum(i => i.Cantidad);

        public async Task InicializarCarritoAsync(ApiService apiService)
        {
            if (CarritoId == 0)
            {
                CarritoId = await apiService.CrearCarritoAsync();
            }
        }

        public void CambiarCantidad(int productoId, int nuevaCantidad)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null && nuevaCantidad > 0 && nuevaCantidad <= item.Producto.Stock)
            {
                item.Cantidad = nuevaCantidad;
            }
        }
        

        public void AgregarProducto(Producto producto)
        {
            var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
            }
        }

        public void Vaciar()
        {
            items.Clear();
        }

        public decimal CalcularTotal()
        {
            return items.Sum(i => i.Producto.Precio * i.Cantidad);
        }
    }
}