namespace servidor.Dtos
{
    public class CompraDto
    {
        public int CarritoId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public List<CompraItemDto> Items { get; set; }
    }

    public class CompraItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
