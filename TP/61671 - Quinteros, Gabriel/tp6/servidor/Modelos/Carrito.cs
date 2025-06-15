namespace servidor.Modelos
{
    public class Carrito
    {
        public Guid Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
    }

    public class ItemCarrito
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Guid CarritoId { get; set; }
        public int Cantidad { get; set; }

        public Producto Producto { get; set; }
        public Carrito Carrito { get; set; }
    }
}