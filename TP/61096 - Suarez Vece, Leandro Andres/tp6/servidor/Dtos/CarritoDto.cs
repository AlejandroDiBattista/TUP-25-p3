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

public class Page
{
    public int pageIndex { get; set; }
    public int pageSize { get; set; }
}
public class CompraPendienteDto
{
    public int Id_compra { get; set; }
    public DateTime Fecha { get; set; }
    public bool Entregado { get; set; }
}
public class CompraGetDto
{
    public int Id_compra { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public DateTime Fecha { get; set; }
    public List<ItemCompraGtDto> Items { get; set; }

    public decimal Total => Items?.Sum(i => i.subTotal) ?? 0m;
}
public class ConfirmarCompraDto
{
    [MaxLength(50)]
    public string? NombreCliente { get; set; }

    [MaxLength(50)]
    public string? ApellidoCliente { get; set; }

    [EmailAddress]
    public string? EmailCliente { get; set; }
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