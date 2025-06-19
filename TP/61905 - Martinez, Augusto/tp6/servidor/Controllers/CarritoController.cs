using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class CarritoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CarritoController> _logger;

    public CarritoController(AppDbContext context, ILogger<CarritoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{usuarioId}")]
    public async Task<ActionResult<Carrito>> ObtenerCarrito(int usuarioId)
    {
        var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
        if (!usuarioExiste)
            return BadRequest("El usuario no existe, no se puede crear el carrito.");

        var carrito = await _context.Carritos
                                    .Include(c => c.CarritoItems)
                                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null)
        {
            carrito = new Carrito
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                ClienteNombre = "Nuevo Cliente",
                ClienteEmail = "cliente@email.com",
                Confirmado = false,
                CarritoItems = new List<CarritoItem>()
            };

            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
        }

        return Ok(carrito);
    }

[HttpPost("{carritoId}/{productoId}")]
public async Task<ActionResult<CarritoItem>> AgregarAlCarrito(Guid carritoId, int productoId)
{
    _logger.LogInformation($"Intentando agregar producto ID {productoId} al carrito ID {carritoId}");

    var carrito = await _context.Carritos.FindAsync(carritoId);
    if (carrito == null)
        return NotFound("Carrito no encontrado.");

    var producto = await _context.Productos.FindAsync(productoId);
    if (producto == null)
        return NotFound("Producto no encontrado.");

    if (producto.Stock <= 0)
        return BadRequest($"No hay stock disponible para '{producto.Nombre}'.");

    var itemExistente = await _context.CarritoItems
                                      .FirstOrDefaultAsync(ci => ci.CarritoId == carritoId && ci.ProductoId == productoId);

    if (itemExistente != null)
    {
        itemExistente.Cantidad++;
    }
    else
    {
        _context.CarritoItems.Add(new CarritoItem
        {
            CarritoId = carritoId,
            ProductoId = productoId,
            Cantidad = 1,
            PrecioUnitario = producto.Precio,
            Nombre = producto.Nombre,
            ImagenUrl = producto.ImagenUrl
        });
    }
    await _context.SaveChangesAsync();
    return Ok("Producto agregado al carrito.");
}

    [HttpGet("contador/{carritoId}")]
    public async Task<ActionResult<int>> ObtenerContadorProductos(Guid carritoId)
    {
        if (carritoId == Guid.Empty)
            return BadRequest("El carritoId proporcionado no es válido.");

        var carrito = await _context.Carritos
                                    .Include(c => c.CarritoItems)
                                    .FirstOrDefaultAsync(c => c.Id == carritoId);

        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        var totalProductos = carrito.CarritoItems.Sum(ci => ci.Cantidad);
        return Ok(totalProductos);
    }

    [HttpDelete("eliminar/{productoId}")]
    public async Task<IActionResult> EliminarProducto(int productoId)
    {
        var item = await _context.CarritoItems.FirstOrDefaultAsync(ci => ci.ProductoId == productoId);
        if (item == null)
            return NotFound("Producto no encontrado en el carrito.");

        _context.CarritoItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{usuarioId}")]
    public async Task<IActionResult> VaciarCarrito(int usuarioId)
    {
        var carrito = await _context.Carritos
                                    .Include(c => c.CarritoItems)
                                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrito == null)
            return NotFound("No se encontró el carrito del usuario.");

        _context.CarritoItems.RemoveRange(carrito.CarritoItems);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
