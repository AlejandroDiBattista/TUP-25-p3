using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servidor.Data;
using Servidor.Models;

namespace Servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarritosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarritosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CrearCarrito()
        {
            var carrito = new Carrito();
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
            return Ok(carrito.Id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<object>>> ObtenerCarrito(string id)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound();

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

    if (producto.Stock < cantidad)
        return BadRequest("Producto sin stock suficiente");

    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (item != null)
    {
        if (producto.Stock < item.Cantidad + cantidad)
            return BadRequest("No hay suficiente stock para incrementar");
        item.Cantidad += cantidad;
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

            decimal total = 0;
            foreach (var item in carrito.Items)
            {
                var producto = item.Producto!;
                if (producto.Stock < item.Cantidad)
                    return BadRequest($"Stock insuficiente para {producto.Nombre}");

                producto.Stock -= item.Cantidad;
                total += item.Cantidad * item.PrecioUnitario;
            }

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
            _context.Carritos.Remove(carrito); // Limpia carrito tras la compra
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Compra confirmada", compra.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> VaciarCarrito(string id)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carrito == null)
                return NotFound();

            carrito.Items.Clear();
            await _context.SaveChangesAsync();
            return NoContent();
        }

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
                carrito.Items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
