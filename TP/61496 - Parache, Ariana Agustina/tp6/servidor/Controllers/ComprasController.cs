using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using servidor.Models;
using servidor.Data;
using Compartido.Models;
using Compartido.Dtos;



[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly TiendaContext _context;

    public ComprasController(TiendaContext context)
    {
        _context = context;
    }

    [HttpPost("confirmar")]
    public async Task<IActionResult> ConfirmarCompra([FromBody] CompraDto compra)
    {
        // Validar campos obligatorios (opcional si usas [Required] en DTO)
        if (string.IsNullOrWhiteSpace(compra.Nombre) ||
            string.IsNullOrWhiteSpace(compra.Apellido) ||
            string.IsNullOrWhiteSpace(compra.Email) ||
            compra.Items == null || !compra.Items.Any())
        {
            return BadRequest(new { Exito = false, Mensaje = "Datos incompletos" });
        }

        // Traer los productos implicados para validar stock
        var productosIds = compra.Items.Select(i => i.ProductoId).ToList();
        var productosDb = await _context.Productos
                              .Where(p => productosIds.Contains(p.Id))
                              .ToListAsync();

        // Validar stock
        foreach (var item in compra.Items)
        {
            var producto = productosDb.FirstOrDefault(p => p.Id == item.ProductoId);
            if (producto == null)
                return BadRequest(new { Exito = false, Mensaje = $"Producto {item.ProductoId} no existe" });

            if (item.Cantidad > producto.Stock)
                return BadRequest(new { Exito = false, Mensaje = $"No hay stock suficiente para {producto.Nombre}" });
        }

   
        var compraDb = new Compra
        {
            Nombre = compra.Nombre,
            Apellido = compra.Apellido,
            Email = compra.Email,
            Fecha = DateTime.Now,
            Items = compra.Items.Select(i => new ItemCompra
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad
            }).ToList()
        };

        _context.Compras.Add(compraDb);

        // Reducir stock de productos
        foreach (var item in compra.Items)
        {
            var producto = productosDb.First(p => p.Id == item.ProductoId);
            producto.Stock -= item.Cantidad;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Exito = true, Mensaje = "Compra confirmada" });
    }
}
