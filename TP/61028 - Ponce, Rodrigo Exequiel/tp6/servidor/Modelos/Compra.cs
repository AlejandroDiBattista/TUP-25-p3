namespace servidor.Modelos
{
    public class Compra
    {
        public int Id { get; set; }
        public List<ItemCompra> Items { get; set; }
        public decimal Total { get; set; }      // <--- Agrega esto
        public DateTime Fecha { get; set; }     // <--- Y esto
        // ...otros campos si tienes...
    }
}
