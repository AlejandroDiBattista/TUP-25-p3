namespace servidor.Modelos
{
    public class CarritoItem
    {
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}