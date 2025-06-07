using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Collections.Generic; // List<T>

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true)
{
    Console.WriteLine("\n=== MENÚ ===");
    Console.WriteLine("1. Lista de todos los productos");
    Console.WriteLine("2. Lista de productos bajo stock");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock de un producto");
    Console.WriteLine("5. Cerrar programa");
    Console.Write("Opcion: ");

    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await MostrarProductos("/productos");
            break;
        case "2":
            await MostrarProductos("/productos/bajo-stock");
            break;
        case "3":
            await ModificarStock("agregar");
            break;
        case "4":
            await ModificarStock("quitar");
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Opción inválida, vuelve a intentarlo.");
            break;
    }
}

async Task MostrarProductos(string endpoint)
{
    var json = await http.GetStringAsync($"{baseUrl}{endpoint}");
    var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    foreach (var p in productos)
    {
        Console.WriteLine($"{p.Id} - {p.Nombre,-15} ${p.Precio,8:0.00} - Stock: {p.Stock}");
    }
}

async Task ModificarStock(string tipo)
{
    Console.Write("Id del producto: ");
    int id = int.Parse(Console.ReadLine()!);
    Console.Write("Cantidad: ");
    int cant = int.Parse(Console.ReadLine()!);

    var url = tipo == "agregar"
        ? $"{baseUrl}/productos/{id}/agregar-stock/{cant}"
        : $"{baseUrl}/productos/{id}/quitar-stock/{cant}";

    var resp = await http.PostAsync(url, null);
    if (resp.IsSuccessStatusCode)
        Console.WriteLine("¡Stock actualizado correctamente!");
    else
        Console.WriteLine("Error: " + await resp.Content.ReadAsStringAsync());
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
