using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Modelos;

[Route("api/carritos")]
[ApiController]
public class CarritoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarritoController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Obtener productos en el carrito
    [HttpGet("{carritoId}/productos")]
    public async Task<ActionResult<List<Producto>>> ObtenerProductosEnCarrito(int carritoId)
    {
        var carrito = await _context.Carritos.Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == carritoId);

        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        return Ok(carrito.Productos);
    }

    // ✅ Crear un nuevo carrito
    [HttpPost]
    public async Task<ActionResult<Carrito>> CrearCarrito()
    {
        var nuevoCarrito = new Carrito();
        _context.Carritos.Add(nuevoCarrito);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerProductosEnCarrito), new { carritoId = nuevoCarrito.Id }, nuevoCarrito);
    }

    // ✅ Agregar producto al carrito
    [HttpPost("{carritoId}/productos/{productoId}")]
    public async Task<IActionResult> AgregarProductoAlCarrito(int carritoId, int productoId)
    {
        var carrito = await _context.Carritos.Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == carritoId);

        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        var producto = await _context.Productos.FindAsync(productoId);
        if (producto == null)
            return NotFound("Producto no encontrado.");

        carrito.Productos.Add(producto);
        await _context.SaveChangesAsync();

        return Ok(carrito.Productos);
    }

    // ✅ Eliminar producto del carrito
    [HttpDelete("{carritoId}/productos/{productoId}")]
    public async Task<IActionResult> EliminarProductoDelCarrito(int carritoId, int productoId)
    {
        var carrito = await _context.Carritos.Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == carritoId);

        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        var producto = carrito.Productos.FirstOrDefault(p => p.Id == productoId);
        if (producto == null)
            return NotFound("Producto no está en el carrito.");

        carrito.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return Ok(carrito.Productos);
    }

    // ✅ Confirmar compra
    [HttpPost("{carritoId}/confirmar")]
    public async Task<IActionResult> ConfirmarCompra(int carritoId)
    {
        var carrito = await _context.Carritos.Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.Id == carritoId);

        if (carrito == null)
            return NotFound("Carrito no encontrado.");

        carrito.Productos.Clear();
        await _context.SaveChangesAsync();

        return Ok("✅ Compra confirmada.");
    }
}