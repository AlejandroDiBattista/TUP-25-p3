using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using System.Linq;
using servidor.Entidades;
[ApiController]
[Route("[controller]")]
public class Controladordeproductos : ControllerBase
{
    private readonly AppDbContext _context;

    public Controladordeproductos(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> Get()
    {
        return await _context.Productos.ToListAsync();
    }
}
