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

while (true)
{
    Console.WriteLine("\n=== MENÚ ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            var productos = await TraerProductosAsync();
            foreach (var p in productos)
                Console.WriteLine($"{p.Id}. {p.Nombre,-20} Stock: {p.Stock}");
            break;

        case "2":
            var bajos = await TraerBajoStockAsync();
            foreach (var p in bajos)
                Console.WriteLine($"{p.Id}. {p.Nombre,-20} Stock: {p.Stock}");
            break;

        case "3":
            Console.Write("ID producto: ");
            int idA = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantA = int.Parse(Console.ReadLine()!);
            await http.PostAsync($"{baseUrl}/productos/agregar?id={idA}&cantidad={cantA}", null);
            Console.WriteLine("Stock agregado.");
            break;

        case "4":
            Console.Write("ID producto: ");
            int idQ = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQ = int.Parse(Console.ReadLine()!);
            var r = await http.PostAsync($"{baseUrl}/productos/quitar?id={idQ}&cantidad={cantQ}", null);
            if (r.IsSuccessStatusCode)
                Console.WriteLine("Stock quitado.");
            else
                Console.WriteLine("Error: Stock insuficiente o ID inválido.");
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Opción no válida.");
            break;
    }
}

async Task<List<Producto>> TraerProductosAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerBajoStockAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}