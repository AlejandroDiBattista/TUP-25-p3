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

    // 🖼️ Asignar imágenes automáticamente según el nombre del producto
    [HttpPost("asignar-imagenes")]
    public async Task<IActionResult> AsignarImagenes()
    {
        var productos = await _context.Productos.ToListAsync();

        foreach (var producto in productos)
        {
            if (string.IsNullOrEmpty(producto.ImagenUrl))
            {
                if (producto.Nombre.Contains("Matraz", StringComparison.OrdinalIgnoreCase))
                    producto.ImagenUrl = "matraz_erlenmeyer.jpg";
                else if (producto.Nombre.Contains("Piseta", StringComparison.OrdinalIgnoreCase))
                    producto.ImagenUrl = "piseta.jpg";
                else if (producto.Nombre.Contains("Termómetro", StringComparison.OrdinalIgnoreCase))
                    producto.ImagenUrl = "termometro.jpg";
                else if (producto.Nombre.Contains("Vaso", StringComparison.OrdinalIgnoreCase))
                    producto.ImagenUrl = "vaso_medidor.jpg";
                else
                    producto.ImagenUrl = "placeholder.png";
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "✅ Imágenes asignadas correctamente." });
    }

    // 🔧 Normalizar nombres de imagen según los archivos reales
    [HttpPost("normalizar-imagenes")]
    public async Task<IActionResult> NormalizarImagenes()
    {
        var productos = await _context.Productos.ToListAsync();

        foreach (var producto in productos)
        {
            if (producto.Nombre.Contains("Matraz", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "matraz_erlenmeyer.jpg";
            else if (producto.Nombre.Contains("Vaso", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "xvas1.jpg";
            else if (producto.Nombre.Contains("Piseta", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "piseta.jpg";
            else if (producto.Nombre.Contains("Balanza", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "balanza_digital.jpg";
            else if (producto.Nombre.Contains("Manta", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "manta_calefactora.jpg";
            else if (producto.Nombre.Contains("Pipeta", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "pipeta_graduada.jpg";
            else if (producto.Nombre.Contains("Tubo", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "tubos_de_ensayo.jpg";
            else if (producto.Nombre.Contains("Agitador", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "agitador_magnetico.jpg";
            else if (producto.Nombre.Contains("Barra", StringComparison.OrdinalIgnoreCase))
                producto.ImagenUrl = "barra_agitacion_magnetica.jpg";
            else
                producto.ImagenUrl = "placeholder.png";
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "✅ Imágenes normalizadas con éxito." });
    }

    // 🧹 Eliminar productos que no tienen nombre o imagen
    [HttpDelete("limpiar-vacios")]
    public async Task<IActionResult> EliminarProductosVacios()
    {
        var productosInvalidos = await _context.Productos
            .Where(p => string.IsNullOrWhiteSpace(p.Nombre) || string.IsNullOrWhiteSpace(p.ImagenUrl))
            .ToListAsync();

        if (!productosInvalidos.Any())
            return Ok("No hay productos vacíos para eliminar.");

        _context.Productos.RemoveRange(productosInvalidos);
        await _context.SaveChangesAsync();

        return Ok($"✅ Se eliminaron {productosInvalidos.Count} productos vacíos.");
    }
}