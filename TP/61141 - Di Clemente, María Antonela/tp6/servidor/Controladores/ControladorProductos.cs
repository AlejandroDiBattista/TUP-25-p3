using Microsoft.AspNetCore.Mvc;
using servidor.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace servidor.Controladores
{
    [ApiController]
    [Route("api/productos")]
    public class ControladorProductos : ControllerBase
    {
        private readonly TiendaContexto _contexto;

        public ControladorProductos(TiendaContexto contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Producto>> ObtenerTodos()
        {
            return Ok(_contexto.Productos.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Producto> ObtenerPorId(int id)
        {
            var producto = _contexto.Productos.Find(id);

            if (producto == null)
                return NotFound(new { mensaje = "No se encontro el producto" });

            return Ok(producto);
        }

        [HttpPost]
        public ActionResult<Producto> CrearProducto([FromBody] Producto nuevoProducto)
        {
            try
            {
                _contexto.Productos.Add(nuevoProducto);
                _contexto.SaveChanges();
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoProducto.Id }, nuevoProducto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar producto:");
                Console.WriteLine(ex.ToString());

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine(ex.InnerException.ToString());
                }

                return BadRequest(new { mensaje = "Error al guardar producto: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarProducto(int id, [FromBody] Producto productoActualizado)
        {
            var productoExistente = _contexto.Productos.Find(id);

            if (productoExistente == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            productoExistente.Nombre = productoActualizado.Nombre;
            productoExistente.Precio = productoActualizado.Precio;
            productoExistente.Stock = productoActualizado.Stock;
            productoExistente.Descripcion = productoActualizado.Descripcion;
            productoExistente.Categoria = productoActualizado.Categoria;

            _contexto.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarProducto(int id)
        {
            var producto = _contexto.Productos.Find(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            _contexto.Productos.Remove(producto);
            _contexto.SaveChanges();

            return NoContent();
        }
    }
}
