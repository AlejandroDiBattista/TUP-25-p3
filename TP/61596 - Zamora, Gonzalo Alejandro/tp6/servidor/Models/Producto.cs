using tp6.Models;               // Para acceder a las clases dentro de tp6.Models
using Microsoft.EntityFrameworkCore;  // Para trabajar con Entity Framework Core (EF Core)
using System;                   // Para tipos básicos como Guid, DateTime
using System.Collections.Generic;  // Para trabajar con colecciones como List

namespace tp6.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } // ej: "nintendo.jpg"
    }
}
