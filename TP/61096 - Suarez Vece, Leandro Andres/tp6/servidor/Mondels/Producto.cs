using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Models;

public class Producto
{
    [Key]
    public int Id_producto { get; set; }
    
    [MaxLength(100)]
    [Required]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(500)]
    [Required]
    public string Descripcion { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }
    
    public int Stock { get; set; }
    
    public string ImagenUrl { get; set; } = string.Empty;

    public List<ItemCompra>? ItemsCompra { get; set; }
}
