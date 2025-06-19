using System.Text.Json.Serialization;

namespace Servidor.Modelo
{
    public class Carrito
    {
        public int Id { get; set; }

        [JsonIgnore]
        public List<ItemCarrito> Items { get; set; } = new();
    }
}
