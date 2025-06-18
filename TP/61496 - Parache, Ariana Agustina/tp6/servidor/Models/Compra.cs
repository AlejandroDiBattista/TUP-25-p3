
namespace servidor.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public string Nombre { get; set; }      // Agregá estas propiedades
        public string Apellido { get; set; }
        public string Email { get; set; }
        public DateTime Fecha { get; set; }

        // Lista de los ítems de esta compra
        public List<ItemCompra> Items { get; set; }
    }
}
