namespace servidor.Modelos
{
    public class ItemsCompra
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int CompraId { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
    }
}