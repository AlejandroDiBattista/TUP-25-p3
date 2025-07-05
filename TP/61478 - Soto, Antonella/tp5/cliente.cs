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

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}


HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

while (true)
{
    Console.WriteLine("\n--- Menú ---");
    Console.WriteLine("1) Listar productos");
    Console.WriteLine("2) Listar productos para reponer");
    Console.WriteLine("3) Agregar stock");
    Console.WriteLine("4) Quitar stock");
    Console.WriteLine("5) Salir");
    Console.Write("Elegí una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarProductosParaReponer();
            break;
        case "3":
            await ModificarStock(true);
            break;
        case "4":
            await ModificarStock(false);
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Opción inválida");
            break;
    }
}

async Task ListarProductos()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos");
    Console.WriteLine("\nProductos disponibles:");
    foreach (var p in productos)
        Console.WriteLine($"ID:{p.Id} - {p.Nombre} - Stock: {p.Stock}");
}

async Task ListarProductosParaReponer()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos/reponer");
    Console.WriteLine("\nProductos para reponer (stock < 3):");
    foreach (var p in productos)
        Console.WriteLine($"ID:{p.Id} - {p.Nombre} - Stock: {p.Stock}");
}

async Task ModificarStock(bool agregar)
{
    Console.Write("Ingrese ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido");
        return;
    }

    Console.Write("Ingrese cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida");
        return;
    }

    string accion = agregar ? "agregar" : "quitar";
    var url = $"/productos/{id}/{accion}?cantidad={cantidad}";

    var response = await client.PostAsync(url, null);

    if (response.IsSuccessStatusCode)
    {
        var productoActualizado = await response.Content.ReadFromJsonAsync<Producto>();
        Console.WriteLine($"Producto actualizado: {productoActualizado.Nombre}, Stock: {productoActualizado.Stock}");
    }
    else
    {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {response.StatusCode}. {error}");
    }
}


// Fin del ejemplo