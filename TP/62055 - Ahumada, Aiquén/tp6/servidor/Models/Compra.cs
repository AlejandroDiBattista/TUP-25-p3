namespace servidor.Models;

public class Compra
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<ItemCompra> Items { get; set; }
}