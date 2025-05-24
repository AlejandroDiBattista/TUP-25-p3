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
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; } // Notar que aquí se necesita `Stock` (no `Precio`) para que coincida con el servidor
}

// Fin del ejemplo

using System.Net.Http.Json;

HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

while (true)
{
    Console.WriteLine("\nSeleccione una opción:");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("5. Salir");

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
            var r1 = await client.PostAsync($"/productos/agregar/{idAdd}/{cantidadAdd}", null);
            Console.WriteLine(r1.IsSuccessStatusCode ? "Stock agregado." : "Error al agregar stock.");
            break;

        case "4":
            Console.Write("Ingrese ID del producto: ");
            var idRemove = Console.ReadLine();
            Console.Write("Ingrese cantidad a quitar: ");
            var cantidadRemove = Console.ReadLine();
            var r2 = await client.PostAsync($"/productos/quitar/{idRemove}/{cantidadRemove}", null);
            Console.WriteLine(r2.IsSuccessStatusCode ? "Stock quitado." : "Error al quitar stock.");
            break;

        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}


