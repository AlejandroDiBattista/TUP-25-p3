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

using System.Net.Http.Json;

HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

while (true)
{
    Console.WriteLine("Seleccione una opción:");
    Console.WriteLine("1. Listar productos\n2. Listar productos a reponer\n3. Agregar stock\n4. Quitar stock\n5. Salir");

    var opcion = Console.ReadLine();
    if (opcion == "5") break;

    switch (opcion)
    {
        case "1":
            var productos = await client.GetFromJsonAsync<List<Producto>>("/productos");
            productos?.ForEach(p => Console.WriteLine($"{p.Id} - {p.Nombre} - Stock: {p.Stock}"));
            break;
        case "2":
            var reposicion = await client.GetFromJsonAsync<List<Producto>>("/productos/reposicion");
            reposicion?.ForEach(p => Console.WriteLine($"{p.Id} - {p.Nombre} - Stock: {p.Stock}"));
            break;
        case "3":
            Console.Write("Ingrese ID del producto: ");
            var idAdd = Console.ReadLine();
            Console.Write("Ingrese cantidad a agregar: ");
            var cantidadAdd = Console.ReadLine();
            await client.PostAsync($"/productos/agregar/{idAdd}/{cantidadAdd}", null);
            break;
        case "4":
            Console.Write("Ingrese ID del producto: ");
            var idRemove = Console.ReadLine();
            Console.Write("Ingrese cantidad a quitar: ");
            var cantidadRemove = Console.ReadLine();
            await client.PostAsync($"/productos/quitar/{idRemove}/{cantidadRemove}", null);
            break;
    }
}
