using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Entidades;
using servidor.DTOs;

namespace servidor.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ControladorCarrito : ControllerBase
{
    private readonly AppDbContext _context;
    public ControladorCarrito(AppDbContext context)
    {
        _context = context;
    }

    [HttpPut("confirmar")]
    public async Task<IActionResult> ConfirmarCompra([FromBody] CompraDTO compraDto)
    {
        var compra = new Compra
        {
            Fecha = DateTime.Now,
            NombreCliente = compraDto.NombreCliente,
            ApellidoCliente = compraDto.ApellidoCliente,
            EmailCliente = compraDto.EmailCliente,
            Total = compraDto.Items.Sum(i => i.Cantidad * i.PrecioUnitario)
        };

        _context.Compras.Add(compra);
        await _context.SaveChangesAsync();

        foreach (var item in compraDto.Items)
        {
            var itemcompra = new ItemCompra
            {
                CompraId = compra.Id,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario
            };

            _context.ItemsCompra.Add(itemcompra);


            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto != null)
            {
                producto.Cantidad -= item.Cantidad;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Compra registrada correctamente." });
    }
    [HttpPost("reservas")]
public async Task<IActionResult> CrearReserva([FromBody] ReservacionDTO reserva)
{
    foreach (var item in reserva.Items)
    {
        var producto = await _context.Productos.FindAsync(item.ProductoId);
        if (producto == null)
        {
            return NotFound(new { mensaje = $"Producto con ID {item.ProductoId} no encontrado." });
        }

        if (producto.Cantidad < item.Cantidad)
        {
            return BadRequest(new { mensaje = $"Stock insuficiente para el producto '{producto.Nombre}'." });
        }

        // Reservar stock
        producto.Cantidad -= item.Cantidad;
    }

    await _context.SaveChangesAsync();
    return Ok(new { mensaje = "Reserva realizada correctamente." });
}

}
