using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(AppDbContext context, ILogger<ProductosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ✅ Obtener todos los productos con validaciones mejoradas
    [HttpGet]
    public async Task<ActionResult<List<Producto>>> GetProductos()
    {
        try
        {
            var conexionOk = await _context.Database.CanConnectAsync();
            if (!conexionOk)
            {
                _logger.LogError("Error de conexión: la base de datos no está disponible.");
                return StatusCode(500, "Error interno: la base de datos no está conectada.");
            }

            var productos = await _context.Productos
                                          .Include(p => p.VentaItems)
                                          .AsNoTracking()
                                          .ToListAsync();

            _logger.LogInformation($"Cantidad de productos obtenidos: {productos.Count}");

            if (!productos.Any())
            {
                _logger.LogWarning("No hay productos disponibles en la base de datos.");
                return NotFound("No hay productos disponibles.");
            }

            return Ok(productos);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError($"Error de base de datos al obtener productos: {dbEx.InnerException?.Message}");
            return StatusCode(500, $"Error crítico en la base de datos: {dbEx.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error crítico al obtener productos: {ex.Message}");
            return StatusCode(500, $"Error interno del servidor al obtener productos: {ex.Message}");
        }
    }

    // ✅ Obtener un producto por ID con validación avanzada
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProducto(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID de producto inválido: {id}");
                return BadRequest("El ID debe ser mayor a 0.");
            }

            var conexionOk = await _context.Database.CanConnectAsync();
            if (!conexionOk)
            {
                _logger.LogError("Error de conexión: la base de datos no está disponible.");
                return StatusCode(500, "Error interno: la base de datos no está conectada.");
            }

            var producto = await _context.Productos
                                         .Include(p => p.VentaItems)
                                         .ThenInclude(v => v.Producto)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                _logger.LogWarning($"No se encontró producto con ID {id}.");
                return NotFound($"No se encontró producto con ID {id}.");
            }

            return Ok(producto);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError($"Error de base de datos al obtener producto {id}: {dbEx.InnerException?.Message}");
            return StatusCode(500, $"Error crítico en la base de datos: {dbEx.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error crítico al obtener producto {id}: {ex.Message}");
            return StatusCode(500, $"Error interno del servidor al obtener el producto: {ex.Message}");
        }
    }
}
