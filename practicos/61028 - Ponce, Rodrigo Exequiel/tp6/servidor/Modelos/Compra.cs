namespace servidor.Modelos
{
    public class Compra
    {
        public int Id { get; set; }
        public List<ItemCompra> Items { get; set; }
        public decimal Total { get; set; }   
        public DateTime Fecha { get; set; }   
      
    }
}
