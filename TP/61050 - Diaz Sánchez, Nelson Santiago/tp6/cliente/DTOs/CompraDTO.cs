namespace cliente.DTOs
{
    public class CompraDTO
    {
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string EmailCliente { get; set; }
        public List<ItemCompraDTO> Items { get; set; }
    }
}