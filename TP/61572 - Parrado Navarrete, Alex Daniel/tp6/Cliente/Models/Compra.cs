using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cliente.Models;

public class Compra
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string NombreCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string ApellidoCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string EmailCliente { get; set; } = string.Empty;

    public List<ItemCompra> Items { get; set; } = new();
}