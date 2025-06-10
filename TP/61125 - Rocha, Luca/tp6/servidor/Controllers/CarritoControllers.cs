using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Server.Data;
using TiendaOnline.Server.Models;

namespace TiendaOnline.Server.Controllers
{
    [ApiController]
    [Route("api/carritos")]
    public class CarritoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        // Crear un carrito vacío
        [HttpPost]
        public async Task<ActionResult<int>> CrearCarrito()
        {
            var compra = new Compra { Fecha = DateTime.Now, Total = 0, Items = new List<ItemCompra>() };
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();
            return Ok(compra.Id);
        }
    }
}
