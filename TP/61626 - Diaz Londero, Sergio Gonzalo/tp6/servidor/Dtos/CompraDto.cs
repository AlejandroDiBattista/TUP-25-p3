namespace Servidor.Dtos
{
    public class CompraDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string EmailCliente { get; set; }
        public decimal Total { get; set; }
        public List<ItemCompraDto> Items { get; set; }
    }
    public class ItemCompraDto
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public ProductoDto Producto { get; set; }
    }
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
