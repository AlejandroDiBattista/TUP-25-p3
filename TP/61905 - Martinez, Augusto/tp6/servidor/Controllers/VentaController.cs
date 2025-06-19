using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class VentasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<VentasController> _logger;

    public VentasController(AppDbContext context, ILogger<VentasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ✅ Confirmar la venta y actualizar stock
    [HttpPost("confirmar")]
    public async Task<IActionResult> ConfirmarCompra([FromBody] Venta venta)
    {
        if (venta?.VentaItems == null || !venta.VentaItems.Any())
            return BadRequest("La venta debe incluir al menos un producto.");

        // Validar stock
        foreach (var item in venta.VentaItems)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto == null)
                return BadRequest($"El producto con ID {item.ProductoId} no existe.");

            if (producto.Stock < item.Cantidad)
                return BadRequest($"Stock insuficiente para el producto '{producto.Nombre}'.");
        }

        // Registrar fecha de venta
        venta.Fecha = DateTime.UtcNow;

        // Registrar venta
        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();

        // Actualizar stock
        foreach (var item in venta.VentaItems)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto != null)
                producto.Stock -= item.Cantidad;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Venta confirmada: {venta.Id} con {venta.VentaItems.Count} items.");

        return Ok(new { mensaje = "Venta registrada correctamente", ventaId = venta.Id });
    }
}
