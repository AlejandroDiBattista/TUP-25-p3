namespace servidor.Models;

public class Compra
//Son los Detalles de la compra
{
    public int Id { get; set; }
    
    //Fecha de la compra
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }

//Son los detalles del cliente
    public string NombreCliente { get; set; } = "";
    public string ApellidoCliente { get; set; } = "";
    public string EmailCliente { get; set; } = "";
//Evita que la lista no sea nula
    public List<ItemCompra> Items { get; set; } = new();
}