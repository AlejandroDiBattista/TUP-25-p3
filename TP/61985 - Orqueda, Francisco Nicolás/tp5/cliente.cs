#r "nuget: System.Net.Http.Json"
#r "nuget: System.Text.Json"
#nullable enable


using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;


var baseUrl = "http://localhost:5000";
var http = new HttpClient();
http.BaseAddress = new Uri(baseUrl);

var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

class Producto {
    public int Id { get; set; }
    public string? Nombre { get; set; }  
    public int Stock { get; set; }
}

bool salir = false;

while (!salir)
{
    Console.WriteLine("\n--- Menú ---");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos que se deben reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");

    string? opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarProductosBajoStock();
            break;
        case "3":
            await ModificarStock(true);
            break;
        case "4":
            await ModificarStock(false);
            break;
        case "0":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

async Task ListarProductos()
{
    var productos = await http.GetFromJsonAsync<List<Producto>>("/productos");
    Console.WriteLine("\n--- Productos ---");
    foreach (var p in productos!)
    {
        Console.WriteLine($"{p.Id}. {p.Nombre} - Stock: {p.Stock}");
    }
}

async Task ListarProductosBajoStock()
{
    var productos = await http.GetFromJsonAsync<List<Producto>>("/productos/reponer");
    Console.WriteLine("\n--- Productos a reponer (stock < 3) ---");
    foreach (var p in productos!)
    {
        Console.WriteLine($"{p.Id}. {p.Nombre} - Stock: {p.Stock}");
    }
}

async Task ModificarStock(bool agregar)
{
    Console.Write("Ingrese el ID del producto: ");
    int id = int.Parse(Console.ReadLine()!);
    Console.Write("Ingrese la cantidad: ");
    int cantidad = int.Parse(Console.ReadLine()!);

    var response = await http.PutAsync(
        $"/productos/{id}/{(agregar ? "agregar" : "quitar")}?cantidad={cantidad}",
        null);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Stock actualizado correctamente.");
    }
    else
    {
        string error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {error}");
    }
}

