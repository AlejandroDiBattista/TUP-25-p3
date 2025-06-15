using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/productos")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // 🔍 Obtener todos los productos
    [HttpGet]
    public async Task<ActionResult<List<Producto>>> GetProductos()
    {
        var productos = await _context.Productos.ToListAsync();

        if (productos == null || productos.Count == 0)
        {
            return NotFound("⚠️ No se encontraron productos en la base de datos.");
        }

        return Ok(productos);
    }

    // ➕ Agregar un nuevo producto
    [HttpPost]
    public async Task<IActionResult> AddProducto([FromBody] Producto nuevoProducto)
    {
        if (nuevoProducto == null)
            return BadRequest("❌ El producto enviado es nulo.");

        _context.Productos.Add(nuevoProducto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductos), new { id = nuevoProducto.Id }, nuevoProducto);
    }

    // ✏️ Actualizar producto existente
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProducto(int id, [FromBody] Producto productoActualizado)
    {
        if (id != productoActualizado.Id)
            return BadRequest("❌ IDs no coinciden, imposible actualizar el producto.");

        _context.Entry(productoActualizado).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Productos.AnyAsync(p => p.Id == id))
                return NotFound("⚠️ Producto no encontrado.");
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
            return NotFound("⚠️ No se encontró el producto que intentas eliminar.");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}