using Cliente.Modelos;

namespace Cliente.Modelos
{
    public class CarritoItem
    {
        public Producto Producto { get; set; } = new();
        public int Cantidad { get; set; }
    }
}