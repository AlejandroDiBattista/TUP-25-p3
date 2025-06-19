using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La ruta base será: /api/productos
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor que recibe el contexto de base de datos por inyección de dependencias
        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: /api/productos?busqueda=xxx
        // Este endpoint devuelve la lista de productos, y permite filtrar por búsqueda opcional
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> Get([FromQuery] string? busqueda)
        {
            // Creamos la consulta base que devuelve todos los productos
            var query = _context.Productos.AsQueryable();

            // Si se proporciona una cadena de búsqueda, filtramos por nombre o descripción
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(busqueda) || p.Descripcion.Contains(busqueda));
            }

            // Ejecutamos la consulta y devolvemos los productos como JSON
            return await query.ToListAsync();
        }
    }
}
