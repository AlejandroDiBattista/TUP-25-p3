using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Dto;

public class ItemCompraDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
public class CompraDto
{
    public DateTime Fecha { get; set; } = DateTime.Now;
}
public class ItemCompraGtDto
{
    public int Id_iten { get; set; }
    public int Cantidad { get; set; }
    public int ProductoId { get; set; }
    public int CompraId { get; set; }
    public string NombreProducto { get; set; }
    public decimal PrecioProducto { get; set; }

    public decimal subTotal => Cantidad * PrecioProducto;
}