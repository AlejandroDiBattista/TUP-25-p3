/*using Microsoft.AspNetCore.Mvc;
using servidor.Modelos;
using servidor.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace servidor.Controllers
{
    [ApiController]
    [Route("carritos")]
    public class CarritoController : ControllerBase
    {
        private static Dictionary<int, Carrito> carritos = new();
        private static int contadorId = 1;

        private readonly TiendaDbContext _context;

        public CarritoController(TiendaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CrearCarrito()
        {
            var nuevoCarrito = new Carrito { Id = contadorId++ };
            carritos[nuevoCarrito.Id] = nuevoCarrito;

            return Ok(new CrearCarritoResponse { Id = nuevoCarrito.Id });
        }

        [HttpPut("{carritoId}/{productoId}")]
        public async Task<IActionResult> AgregarProducto(int carritoId, int productoId, [FromBody] Dictionary<string, int> body)
        {
            if (!body.TryGetValue("cantidad", out int cantidad))
                return BadRequest("Falta la cantidad.");

            if (!carritos.TryGetValue(carritoId, out var carrito))
                return NotFound("Carrito no encontrado.");

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
                return NotFound("Producto no encontrado en la base de datos.");

            var itemExistente = carrito.Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItem
                {
                    Producto = producto,
                    Cantidad = cantidad
                });
            }

            return Ok();
        }

        [HttpGet("{carritoId}")]
        public IActionResult ObtenerCarrito(int carritoId)
        {
            if (!carritos.TryGetValue(carritoId, out var carrito))
                return NotFound("Carrito no encontrado.");

            return Ok(carrito);
        }
    }

    public class CrearCarritoResponse
    {
        public int Id { get; set; }
    }
}
*/