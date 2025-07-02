using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

Console.WriteLine("=== Productos ===");
foreach (var p in await TraerAsync()) {
    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c}");
}
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}

// Fin del ejemplo