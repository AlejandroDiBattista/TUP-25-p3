using Microsoft.AspNetCore.Mvc; // Permite usar ControllerBase y atributos como [ApiController] y [Route]
using servidor.Data; // Permite el uso de AppDbContext
using servidor.Models; // Permite el uso de modelos como Producto, Carrito, Venta
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // Obtener todos los productos
    [HttpGet]
    public async Task<ActionResult<List<Producto>>> GetProductos()
    {
        return await _context.Productos.ToListAsync();
    }

    // Obtener un producto por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();
        return producto;
    }
}
