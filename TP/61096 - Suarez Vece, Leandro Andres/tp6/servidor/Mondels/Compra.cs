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

    [MaxLength(50)]
    public string? NombreCliente { get; set; }

    [MaxLength(50)]
    public string? ApellidoCliente { get; set; }

    [EmailAddress]
    public string? EmailCliente { get; set; }

    public List<ItemCompra>? Items { get; set; }

    [NotMapped]
    public decimal Total => Items?.Sum(i => i.Cantidad * i.PrecioUnitario) ?? 0m;
}