namespace servidor.Modelos
{
    using System.Collections.Generic;
    public class NuevaCompraDto
    {
        // public IList<CarritoDto> CarritoDtos { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string CarritoId { get; set; }
    }
    
}