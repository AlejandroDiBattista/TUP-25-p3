using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace servidor.Models
{
    public class Carrito
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string ClienteNombre { get; set; } = string.Empty;

        [Required]
        public string ClienteEmail { get; set; } = string.Empty;

        public bool Confirmado { get; set; } = false;

        // Relación 1:N con CarritoItem
        public List<CarritoItem> Items { get; set; } = new();
    }
}

