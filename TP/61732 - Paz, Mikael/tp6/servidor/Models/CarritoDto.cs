namespace servidor.Models
{
    public class CarritoDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<ItemCarritoDto> Items { get; set; } = new();
    }
    public class ItemCarritoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
