using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace servidor.Models
{
    public class Compra
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public List<ItemCarrito> Items { get; set; }
        public DateTime Fecha { get; set; }
    }
}