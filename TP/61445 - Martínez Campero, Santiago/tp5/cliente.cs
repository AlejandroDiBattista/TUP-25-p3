using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task MainMenu()
{
    while (true)
    {
        Console.WriteLine("\n=== Tienda de Productos ===");
        Console.WriteLine("1. Listar productos disponibles");
        Console.WriteLine("2. Listar productos a reponer");
        Console.WriteLine("3. Agregar stock a un producto");
        Console.WriteLine("4. Quitar stock a un producto");
        Console.WriteLine("5. Salir");
        Console.Write("Seleccione una opción: ");
        var opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                await ListarProductosDisponibles();
                break;
            case "2":
                await ListarProductosAReponer();
                break;
            case "3":
                await AgregarStock();
                break;
            case "4":
                await QuitarStock();
                break;
            case "5":
                return;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }
}

async Task ListarProductosDisponibles(){
    Console.WriteLine("\n=== Productos Disponibles ===");
    var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos", jsonOpt);
    if (productos is null || !productos.Any())
    {
        Console.WriteLine("No hay productos disponibles.");
        return;
    }
    foreach (var p in productos) {
        Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} Stock: {p.Stock}");
    }
}

async Task ListarProductosAReponer(){
    Console.WriteLine("\n=== Productos a Reponer (Stock < 3) ===");
    var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/reponer", jsonOpt);
    if (productos is null || !productos.Any())
    {
        Console.WriteLine("No hay productos para reponer.");
        return;
    }
    foreach (var p in productos) {
        Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} Stock: {p.Stock}");
    }
}

async Task AgregarStock(){
    Console.Write("Ingrese el ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido.");
        return;
    }
    Console.Write("Ingrese la cantidad a agregar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad no válida.");
        return;
    }

    var response = await http.PostAsJsonAsync($"{baseUrl}/productos/{id}/agregar-stock", new StockDto { Cantidad = cantidad }, jsonOpt);
    if (response.IsSuccessStatusCode)
    {
        var producto = await response.Content.ReadFromJsonAsync<Producto>(jsonOpt);
        Console.WriteLine($"Stock agregado. Nuevo stock de {producto?.Nombre}: {producto?.Stock}");
    }
    else
    {
        Console.WriteLine($"Error al agregar stock: {response.ReasonPhrase}");
    }
}
