using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cliente.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string ApellidoCliente { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
        public string EmailCliente { get; set; }

        public List<ItemCompra> Items { get; set; } = new();
    }
}
