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

        // recibe el contexto de la base de datos y lo guarda para usarlo luego
        public ControladorProductos(TiendaContexto contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
         // Devuelve todos los productos de la base de datos
        public ActionResult<IEnumerable<Producto>> ObtenerTodos()
        {
            return Ok(_contexto.Productos.ToList());
        }
        // Método para manejar la solicitud GET
        [HttpGet("{id}")]
        public ActionResult<Producto> ObtenerPorId(int id)
        {
            var producto = _contexto.Productos.Find(id);

            if (producto == null)
                return NotFound(new { mensaje = "No se encontro el producto" });

            return Ok(producto);
        }

    }
}
