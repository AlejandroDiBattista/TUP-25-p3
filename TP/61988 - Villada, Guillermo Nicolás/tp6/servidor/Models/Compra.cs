using System;
using System.Collections.Generic;

namespace servidor.Models;

public class Compra
{
    public Guid Id { get; set; }
    public Cliente Cliente { get; set; }
    public List<ItemCarrito> Items { get; set; }
    public DateTime Fecha { get; set; }
}