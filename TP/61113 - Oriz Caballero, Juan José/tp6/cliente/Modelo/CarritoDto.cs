namespace Cliente.Modelo
{
    #nullable enable
    public class CarritoDto
    {
        public int Id { get; set; }
        public List<ItemCarritoDto> Items { get; set; } = new();
    }
}
