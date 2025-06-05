using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Dto;

public class CarritoDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
public class CarritoGtDto
{
    public int Id_Carrito { get; set; }
    public int Cantidad { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; }
    public decimal PrecioProducto { get; set; }

    public decimal subTotal => Cantidad * PrecioProducto;
}