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
    Console.WriteLine("\n1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var op = Console.ReadLine();

    switch (op)
    {
        case "1":
            var productos = await http.GetFromJsonAsync<List<Producto>>("/productos");
            productos?.ForEach(p => Console.WriteLine($"{p.Id}: {p.Name} - Stock: {p.Stock}"));
            break;

        case "2":
            var bajos = await http.GetFromJsonAsync<List<Producto>>("/productos/reponer");
            bajos?.ForEach(p => Console.WriteLine($"{p.Id}: {p.Name} - Stock: {p.Stock}"));
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idAgregar = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantAgregar = int.Parse(Console.ReadLine()!);
            var r1 = await http.PostAsync($"/productos/{idAgregar}/agregar?cantidad={cantAgregar}", null);
            Console.WriteLine(r1.IsSuccessStatusCode ? "Stock agregado" : await r1.Content.ReadAsStringAsync());
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idQuitar = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQuitar = int.Parse(Console.ReadLine()!);
            var r2 = await http.PostAsync($"/productos/{idQuitar}/quitar?cantidad={cantQuitar}", null);
            Console.WriteLine(r2.IsSuccessStatusCode ? "Stock reducido" : await r2.Content.ReadAsStringAsync());
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Stock { get; set; }
}
