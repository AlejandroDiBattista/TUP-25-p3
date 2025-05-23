using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
bool running = true;

while (running)
{
    Console.WriteLine("\n=== MENU ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Elija opción: ");

    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarProductosReponer();
            break;
        case "3":
            await ModificarStock(true);
            break;
        case "4":
            await ModificarStock(false);
            break;
        case "0":
            running = false;
            break;
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
        Console.WriteLine($"ID: {p.Id}, Nombre: {p.Nombre}, Stock: {p.Stock}");
}

async Task ListarProductosReponer()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos/reponer");
    Console.WriteLine("\nProductos a reponer (stock < 3):");
    foreach (var p in productos)
        Console.WriteLine($"ID: {p.Id}, Nombre: {p.Nombre}, Stock: {p.Stock}");
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

    var url = agregar ? $"/productos/{id}/agregarstock?cantidad={cantidad}" : $"/productos/{id}/quitastock?cantidad={cantidad}";

    var response = await client.PutAsync(url, null);

    if (response.IsSuccessStatusCode)
    {
        var productoActualizado = await response.Content.ReadFromJsonAsync<Producto>();
        Console.WriteLine($"Operación exitosa. Nuevo stock de {productoActualizado.Nombre}: {productoActualizado.Stock}");
    }
    else
    {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {error}");
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}
