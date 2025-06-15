using System.ComponentModel.DataAnnotations;

namespace servidor.Modelos;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }

    [Required]
    public string NombreCliente { get; set; }

    [Required]
    public string ApellidoCliente { get; set; }

    [Required, EmailAddress]
    public string EmailCliente { get; set; }

    public List<ItemCompra> Items { get; set; } = new();
}