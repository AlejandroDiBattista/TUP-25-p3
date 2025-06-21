using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ruta base: /api/carritos
    public class CarritosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarritosController(AppDbContext context)
        {
            _context = context;
        }

        // Crea un nuevo carrito vacío en la base de datos
        [HttpPost]
        public async Task<ActionResult<string>> CrearCarrito()
        {
            var carrito = new Carrito();
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
            return Ok(carrito.Id);
            
    Console.WriteLine($"🛒 Carrito creado con ID: {carrito.Id}"); 

    return Ok(carrito.Id);
        }

        // Obtiene el contenido del carrito con sus productos
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<object>>> ObtenerCarrito(string id)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)               // Incluye ítems del carrito
                .ThenInclude(i => i.Producto)        // Incluye datos del producto
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound();

            // Retorna solo los datos necesarios
            var resultado = carrito.Items.Select(i => new {
                i.Id,
                i.ProductoId,
                Producto = i.Producto == null ? null : new {
                    i.Producto.Id,
                    i.Producto.Nombre,
                    i.Producto.Precio,
                    i.Producto.ImagenUrl
                },
                i.Cantidad,
                i.PrecioUnitario
            });

            return Ok(resultado);
        }

        // Agrega o actualiza un producto dentro del carrito, actualizando stock inmediatamente
        [HttpPut("{id}/{productoId}")]
        public async Task<IActionResult> AgregarProducto(string id, int productoId, [FromQuery] int cantidad)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound("Carrito no encontrado");

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
                return NotFound("Producto no encontrado");

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

            int cantidadAnterior = item?.Cantidad ?? 0;
            int nuevaCantidad = cantidadAnterior + cantidad;

            int diferencia = nuevaCantidad - cantidadAnterior;

            if (producto.Stock < diferencia)
                return BadRequest("Producto sin stock suficiente");

            producto.Stock -= diferencia;

            if (item != null)
            {
                item.Cantidad = nuevaCantidad;
            }
            else
            {
                carrito.Items.Add(new ItemCarrito
                {
                    ProductoId = producto.Id,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producto agregado correctamente" });
        }

        // Confirma la compra, ya que stock fue descontado antes, solo registra compra y elimina carrito
        [HttpPut("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarCompra(string id, [FromBody] Compra datos)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound();

            if (!carrito.Items.Any())
                return BadRequest("El carrito está vacío");

            decimal total = carrito.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

            var compra = new Compra
            {
                NombreCliente = datos.NombreCliente,
                ApellidoCliente = datos.ApellidoCliente,
                EmailCliente = datos.EmailCliente,
                Fecha = DateTime.Now,
                Total = total,
                ItemsCompra = carrito.Items.Select(i => new ItemCompra
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario
                }).ToList()
            };

            _context.Compras.Add(compra);
            _context.Carritos.Remove(carrito); // Elimina el carrito tras la compra
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Compra confirmada", compra.Id });
        }

        // Elimina todos los productos del carrito y devuelve el stock
        [HttpDelete("{id}")]
        public async Task<IActionResult> VaciarCarrito(string id)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound();

            // Devolver stock de cada producto
            foreach (var item in carrito.Items)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto != null)
                {
                    producto.Stock += item.Cantidad;
                }
            }

            carrito.Items.Clear();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Quita un producto del carrito y devuelve el stock
        [HttpDelete("{id}/{productoId}")]
        public async Task<IActionResult> QuitarProducto(string id, int productoId)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound();

            var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto != null)
                {
                    producto.Stock += item.Cantidad;
                }

                carrito.Items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
