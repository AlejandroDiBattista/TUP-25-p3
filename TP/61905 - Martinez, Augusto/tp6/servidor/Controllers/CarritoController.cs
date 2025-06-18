using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CarritoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarritoController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Obtener los productos en el carrito por CarritoId (NO por UsuarioId)
    [HttpGet("{carritoId}")]
    public async Task<ActionResult<List<CarritoItem>>> GetCarrito(Guid carritoId)
    {
        var carrito = await _context.Carritos
                                    .Include(c => c.CarritoItems)
                                    .ThenInclude(ci => ci.Producto)
                                    .FirstOrDefaultAsync(c => c.Id == carritoId);

        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        return Ok(carrito.CarritoItems);
    }

    // ✅ Agregar un producto al carrito
    [HttpPost]
    public async Task<ActionResult<CarritoItem>> AgregarAlCarrito(CarritoItem item)
    {
        var carrito = await _context.Carritos.FindAsync(item.CarritoId);
        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        _context.CarritoItems.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCarrito), new { carritoId = item.CarritoId }, item);
    }
}
