using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public IActionResult GetProductos()
    {
        return Ok(_context.Productos.ToList());
    }
}