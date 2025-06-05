using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor.Dto;

public class CarritoDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}