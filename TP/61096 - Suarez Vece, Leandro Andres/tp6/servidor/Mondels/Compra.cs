using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Servidor.Models;

public class Compra
{
    [Key]
    public int Id_compra { get; set; }
    public DateTime Fecha { get; set; }
    [Required]
    [MaxLength(50)]
    public string NombreCliente { get; set; }
    [Required]
    [MaxLength(50)]
    public string ApellidoCliente { get; set; }
    [Required]
    [EmailAddress]
    public string EmailCliente { get; set; }

    public ICollection<ItemCompra>? Items { get; set; }

    [NotMapped]
    public decimal Total => Items?.Sum(i => i.Cantidad * i.PrecioUnitario) ?? 0m;
}