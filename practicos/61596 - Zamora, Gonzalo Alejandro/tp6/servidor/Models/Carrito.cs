using tp6.Models;               // Para acceder a las clases dentro de tp6.Models
using Microsoft.EntityFrameworkCore;  // Para trabajar con Entity Framework Core (EF Core)
using System;                   // Para tipos básicos como Guid, DateTime
using System.Collections.Generic;  // Para trabajar con colecciones como List
namespace tp6.Models
{
    using tp6.Models;
    public class Carrito
    {
        public Guid CarritoId { get; set; }
        public List<CarritoItem> CarritoItems { get; set; }
    }
}
