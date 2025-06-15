namespace servidor.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<CarritoItem> Items { get; set; } = new();
    }

    public class CarritoItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
