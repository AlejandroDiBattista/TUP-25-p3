using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

bool salir = false;
while (!salir)
{
    Console.WriteLine("\n=== MENÚ ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await Listar("/productos");
            break;
        case "2":
            await Listar("/reposicion");
            break;
        case "3":
            await ModificarStock("agregar-stock");
            break;
        case "4":
            await ModificarStock("quitar-stock");
            break;
        case "0":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción inválida");
            break;
    }
}

async Task Listar(string ruta)
{
    try {
        var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}{ruta}", jsonOpt);
        Console.WriteLine("\nID  Nombre                Precio    Stock");
        foreach (var p in productos!) {
            Console.WriteLine($"{p.Id,2}  {p.Nombre,-20} {p.Precio,8:C} {p.Stock,6}");
        }
    } catch {
        Console.WriteLine("Error al obtener los productos.");
    }
}

async Task ModificarStock(string accion)
{
    Console.Write("Ingrese ID de producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;
    Console.Write("Ingrese cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad)) return;

    var url = $"{baseUrl}/{accion}?id={id}&cantidad={cantidad}";
    var response = await http.PostAsync(url, null);

    if (response.IsSuccessStatusCode) {
        var producto = await response.Content.ReadFromJsonAsync<Producto>(jsonOpt);
        Console.WriteLine($"Nuevo stock de '{producto!.Nombre}': {producto.Stock}");
    } else {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {error}");
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
