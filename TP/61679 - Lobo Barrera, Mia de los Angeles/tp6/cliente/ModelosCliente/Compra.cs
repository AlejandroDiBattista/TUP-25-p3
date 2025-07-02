using System.ComponentModel.DataAnnotations;
using cliente.Modelos;
using static cliente.Pages.Confirmacion;

namespace cliente.Modelos
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }

        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;

        public List<ItemCompra> Items { get; set; } = new ();
    }
}