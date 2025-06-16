using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Models;

public class Carrito
{
    [Key]
    public int Id_Carrito { get; set; }
    public int Cantidad { get; set; }

    [ForeignKey(nameof(ProductoId))]
    public int ProductoId { get; set; }

    public List<Producto> productos { get; set; }
}