namespace TiendaOnline.Servidor.Models;
public class Compra
{
    public int Id { get; set; }
    public System.DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
}