using System.Text.Json.Serialization;

namespace cliente.Models
{
    public class DetalleCarritoMemoria
    {
        [JsonPropertyName("articuloId")]
        public int ArticuloId { get; set; }


        [JsonPropertyName("unidades")]
        public int Unidades { get; set; }

        [JsonPropertyName("valorPorUnidad")]
        public double ValorPorUnidad { get; set; }
    }
}
