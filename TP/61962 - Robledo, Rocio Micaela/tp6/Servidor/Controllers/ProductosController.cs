using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Controllers
{
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
        public async Task<ActionResult<IEnumerable<Producto>>> Get([FromQuery] string? busqueda)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(busqueda) || p.Descripcion.Contains(busqueda));
            }

            return await query.ToListAsync();
        }
    }
}
