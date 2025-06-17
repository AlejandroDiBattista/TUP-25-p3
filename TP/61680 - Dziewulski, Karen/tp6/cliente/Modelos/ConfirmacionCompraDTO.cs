namespace cliente.Modelos
{
    public class CompraDTO
    {
        public ClienteDto Cliente { get; set; } = new();
        public List<ItemDTO> Items { get; set; } = new();
    }

    public class ClienteDto
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class ItemDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
