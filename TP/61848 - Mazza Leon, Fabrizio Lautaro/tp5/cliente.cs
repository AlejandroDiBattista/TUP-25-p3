using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true)
{
    Console.WriteLine("\n=== MENÚ STOCK DE PRODUCTOS ===");
    Console.WriteLine("\n1. Listar todos los productos");
    Console.WriteLine("2. Listar productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("\nOpción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarAReponer();
            break;
        case "3":
            await ModificarStock("agregar");
            break;
        case "4":
            await ModificarStock("quitar");
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

async Task ListarProductos()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    Console.WriteLine("\n=== Productos ===");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id,2} {p.Nombre,-20} {p.Precio,8:c} Stock: {p.Stock}");
}

async Task ListarAReponer()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    Console.WriteLine("\n=== Productos a reponer ===");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id,2} {p.Nombre,-20} Stock: {p.Stock}");
}

async Task ModificarStock(string accion)
{
    Console.Write("\nID del producto: ");
    int id = int.Parse(Console.ReadLine()!);
    Console.Write("\nCantidad: ");
    int cantidad = int.Parse(Console.ReadLine()!);

    var url = $"{baseUrl}/productos/{accion}/{id}/{cantidad}";
    var resp = await http.PostAsync(url, null);

    if (resp.IsSuccessStatusCode)
    {
        var json = await resp.Content.ReadAsStringAsync();
        var p = JsonSerializer.Deserialize<Producto>(json, jsonOpt)!;
        Console.WriteLine($"\n{(accion == "agregar" ? "Agregado" : "Quitado")} correctamente. Nuevo stock: {p.Stock}");
    }
    else
    {
        var msg = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {resp.StatusCode} - {msg}");
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}