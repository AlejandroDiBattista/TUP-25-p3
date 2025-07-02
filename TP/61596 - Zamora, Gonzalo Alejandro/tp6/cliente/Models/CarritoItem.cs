using tp6.Models;               // Para acceder a las clases dentro de tp6.Models
  // Para trabajar con Entity Framework Core (EF Core)
using System;                   // Para tipos básicos como Guid, DateTime
using System.Collections.Generic;  // Para trabajar con colecciones como List

namespace tp6.Models
{
  
    public class CarritoItem
    {
        public int ProductoId { get; set; }
        public string? Nombre { get; set; }          // <- reemplaza Producto.Nombre
        public decimal Precio { get; set; }          // <- reemplaza Producto.Precio
        public int Cantidad { get; set; }

        public decimal Subtotal => Precio * Cantidad;
    
    }}