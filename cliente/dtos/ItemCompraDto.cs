namespace cliente.Dtos
{
    public class ItemCompraDto
    {
        public int ProductoId { get; set; }
        public ProductoDto? Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe => Cantidad * PrecioUnitario;
    }
}
