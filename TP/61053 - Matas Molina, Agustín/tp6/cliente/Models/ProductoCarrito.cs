using TuProyecto.Models;

namespace TuProyecto.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ImagenUrl { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class ProductoCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}