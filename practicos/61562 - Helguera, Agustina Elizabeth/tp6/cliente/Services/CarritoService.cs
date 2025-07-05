using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        public List<ItemCompra> Items { get; private set; } = new();

        public void AgregarProducto(Producto producto)
        {
            var itemExistente = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                Items.Add(new ItemCompra
                {
                    Producto = producto,
                    Cantidad = 1
                });
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
        }

        public decimal ObtenerTotal()
        {
            return Items.Sum(i => i.Cantidad * i.Producto.Precio);
        }
    }
}
