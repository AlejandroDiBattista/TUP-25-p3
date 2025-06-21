#r "nuget: System.Net.Http.Json, 6.0.0"
using System;
using System.Net.Http;
using System.Net.Http.Json; // <--- esta línea habilita GetFromJsonAsync
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true)
{
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos con bajo stock (< 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Elija una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductosAsync();
            break;
        case "2":
            await ListarBajoStockAsync();
            break;
        case "3":
            await ModificarStockAsync(agregar: true);
            break;
        case "4":
            await ModificarStockAsync(agregar: false);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

async Task ListarProductosAsync()
{
    var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos", jsonOpt);
    Console.WriteLine("\n=== Lista de Productos ===");
    foreach (var p in productos!)
        Console.WriteLine($"{p.Id,2} {p.Nombre,-20} {p.Precio,10:C}  Stock: {p.Stock}");
}

async Task ListarBajoStockAsync()
{
    var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/bajo-stock", jsonOpt);
    Console.WriteLine("\n=== Productos con Bajo Stock ===");
    foreach (var p in productos!)
        Console.WriteLine($"{p.Id,2} {p.Nombre,-20} Stock: {p.Stock}");
}

async Task ModificarStockAsync(bool agregar)
{
    Console.Write("ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    Console.Write("Cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad))
    {
        Console.WriteLine("Cantidad inválida.");
        return;
    }

    var url = $"{baseUrl}/productos/{id}/" + (agregar ? "agregar-stock" : "quitar-stock") + $"?cantidad={cantidad}";
    var response = await http.PostAsync(url, null);

    if (response.IsSuccessStatusCode)
        Console.WriteLine(" Operación exitosa.");
    else
        Console.WriteLine($" Error: {await response.Content.ReadAsStringAsync()}");
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}