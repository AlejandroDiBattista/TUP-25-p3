namespace servidor
{
    public class Carrito
    {
        public Guid Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}