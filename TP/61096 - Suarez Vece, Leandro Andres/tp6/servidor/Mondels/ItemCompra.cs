using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Models;

public class ItemCompra
{
    [Key]
    public int Id_iten { get; set; }

    public int Cantidad { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; }


    [ForeignKey(nameof(ProductoId))]
    public int ProductoId { get; set; }


    [ForeignKey(nameof(CompraId))]
    public int CompraId { get; set; }
    public Compra? Compra { get; set; }
    public Producto? Producto { get; set; }


}
