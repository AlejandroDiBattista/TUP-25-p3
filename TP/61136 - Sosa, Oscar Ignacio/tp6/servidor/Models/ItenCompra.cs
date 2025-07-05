using tp6.Models;               // Para acceder a las clases dentro de tp6.Models
using Microsoft.EntityFrameworkCore;  // Para trabajar con Entity Framework Core (EF Core)
using System;                   // Para tipos básicos como Guid, DateTime
using System.Collections.Generic;  // Para trabajar con colecciones como List
namespace tp6.Models
{
    public class ItemCompra
{
    public int Id { get; set; }
    public int Cantidad{get;set;}
    public decimal PrecioUnitario{get;set;}
    
    // Claves foráneas
    public Guid CompraId { get; set; }
    public int ProductoId { get; set; }
    
    // Propiedades de navegación (relaciones)
    public Compra Compra { get; set; }       // Relación con Compra
    public Producto Producto { get; set; }   // Relación con Producto
}
}

