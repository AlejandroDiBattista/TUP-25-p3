namespace cliente.Models
{
    public class Carrito
    {
        public string Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
    }

    public class ItemCarrito
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

}