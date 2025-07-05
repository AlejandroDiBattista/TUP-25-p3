using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cliente.Models
{
    public class Compra
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? NombreCliente { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string? ApellidoCliente { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? EmailCliente { get; set; }

        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<ItemCompra>? Items { get; set; }
    }
}
