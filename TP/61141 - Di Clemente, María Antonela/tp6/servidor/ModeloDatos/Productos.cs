using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace servidor.ModeloDatos;

public class Producto
{
      public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es obligatorio")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    public string ImagenUrl { get; set; }

    public string Categoria { get; set; }


    public List<ItemCompra> ItemsCompra { get; set; } = new List<ItemCompra>();
}    


