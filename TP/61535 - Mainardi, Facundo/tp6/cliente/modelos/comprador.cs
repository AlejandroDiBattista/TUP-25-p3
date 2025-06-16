using Cliente.Modelos;
using System.ComponentModel.DataAnnotations;

namespace Cliente.Modelos;
    public class Comprador
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
    }
     public class ConfirmacionCompraDto
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public List<ItemCarrito> Items { get; set; }
    }