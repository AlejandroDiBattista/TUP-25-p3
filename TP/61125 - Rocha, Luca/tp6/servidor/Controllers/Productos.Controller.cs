using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Modelos;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // 🔍 Obtener todos los productos
    [HttpGet]
    public IActionResult GetProductos()
    {
        return Ok(_context.Productos.ToList());
    }

    // ➕ Agregar un nuevo producto
    [HttpPost]
    public async Task<IActionResult> AddProducto([FromBody] Producto nuevoProducto)
    {
        if (nuevoProducto == null)
            return BadRequest();

        _context.Productos.Add(nuevoProducto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductos), new { id = nuevoProducto.Id }, nuevoProducto);
    }

    // ✏️ Actualizar producto existente
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProducto(int id, [FromBody] Producto productoActualizado)
    {
        if (id != productoActualizado.Id)
            return BadRequest();

        _context.Entry(productoActualizado).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Productos.Any(p => p.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // ❌ Eliminar un producto
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
            return NotFound();

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}