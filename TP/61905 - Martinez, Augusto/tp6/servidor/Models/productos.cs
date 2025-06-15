namespace servidor.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty; // 🔥 Imagen representativa

        // 🔥 Relación con ventas
        public List<VentaItem> VentaItems { get; set; } = new List<VentaItem>();

        // 🔹 Método para reducir stock de forma segura
        public void ReducirStock(int cantidad)
        {
            if (cantidad > 0 && Stock >= cantidad)
            {
                Stock -= cantidad;
            }
        }
    }
}
