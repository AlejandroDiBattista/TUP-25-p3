using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class Carrito
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey(nameof(Usuario))]
        public int UsuarioId { get; set; } // ✅ Relación con Usuario

        [Required]
        [MaxLength(100)]
        public string ClienteNombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string ClienteEmail { get; set; } = string.Empty;

        public bool Confirmado { get; set; } = false;

        // ✅ Relación 1:N con CarritoItem
        public List<CarritoItem> CarritoItems { get; set; } = new List<CarritoItem>(); 

        // ✅ Propiedad de navegación con Usuario
        public Usuario Usuario { get; set; } = null!;
    }
}
