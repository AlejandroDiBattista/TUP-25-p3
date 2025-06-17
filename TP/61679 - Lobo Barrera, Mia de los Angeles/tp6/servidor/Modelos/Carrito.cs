using servidor.Modelos;
namespace servidor.Modelos
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }
}