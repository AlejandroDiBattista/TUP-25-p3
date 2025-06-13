
using Microsoft.AspNetCore.Mvc;
using servidor.Models;

namespace servidor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Producto>> Get()
    {
        return _context.Productos.ToList();
    }
}