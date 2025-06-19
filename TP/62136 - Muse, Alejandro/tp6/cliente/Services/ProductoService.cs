using cliente.Models;

namespace cliente.Services
{
    public class ProductoService
    {
        public List<Producto> Productos { get; set; } = new();

        public void SetProductos(List<Producto> productos)
        {
            Productos = productos;
        }
    }
}