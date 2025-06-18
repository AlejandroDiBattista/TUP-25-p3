using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class VentasController : ControllerBase
{
    private readonly AppDbContext _context;

    public VentasController(AppDbContext context)
    {
        _context = context;
    }

    // Confirmar la compra y actualizar el stock
    [HttpPost]
    public async Task<ActionResult<Venta>> ConfirmarCompra([FromBody] Venta venta)
    {
        // Validar que la venta tenga productos
        if (venta.VentaItems == null || venta.VentaItems.Count == 0)
        {
            return BadRequest("La venta debe incluir al menos un producto.");
        }

        // Validar stock antes de confirmar la venta
        foreach (var item in venta.VentaItems)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto == null)
            {
                return BadRequest($"El producto con ID {item.ProductoId} no existe.");
            }
            if (producto.Stock < item.Cantidad)
            {
                return BadRequest($"Stock insuficiente para el producto '{producto.Nombre}'.");
            }
        }

        // Registrar la venta
        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();

        // Actualizar stock después de registrar la venta
        foreach (var item in venta.VentaItems)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto != null)
            {
                producto.Stock -= item.Cantidad;
            }
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ConfirmarCompra), new { id = venta.Id }, venta);
    }
}
