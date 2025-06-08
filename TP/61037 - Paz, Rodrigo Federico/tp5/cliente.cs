using System;                     
using System.Linq;                
using System.Net.Http;            
using System.Text;
using System.Text.Json;           
using System.Threading.Tasks;     
using System.Collections.Generic; 

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};


class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

async Task ListarProductosAsync()
{
    try
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);
        Console.WriteLine("\n=== Productos Disponibles ===");
        if (productos != null && productos.Any())
        {
            foreach (var p in productos.OrderBy(p => p.Nombre))
            {
                Console.WriteLine($"ID: {p.Id}, Nombre: {p.Nombre,-20}, Precio: {p.Precio,10:c}, Stock: {p.Stock}");
            }
        }
        else
        {
            Console.WriteLine("No hay productos disponibles.");
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Error al listar productos: {e.Message}");
    }
}

async Task ListarProductosAReponerAsync()
{
    try
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);
        Console.WriteLine("\n=== Productos a Reponer (Stock < 3) ===");
        if (productos != null && productos.Any())
        {
            foreach (var p in productos.OrderBy(p => p.Nombre))
            {
                Console.WriteLine($"ID: {p.Id}, Nombre: {p.Nombre,-20}, Precio: {p.Precio,10:c}, Stock: {p.Stock}");
            }
        }
        else
        {
            Console.WriteLine("No hay productos que necesiten reposición.");
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Error al listar productos a reponer: {e.Message}");
    }
}

async Task AgregarStockAsync()
{
    Console.Write("\nIngrese el ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    Console.Write("Ingrese la cantidad a agregar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida.");
        return;
    }

    try
    {
        var response = await http.PostAsync($"{baseUrl}/productos/agregarstock/{id}/{cantidad}", null);
        if (response.IsSuccessStatusCode)
        {
            var productoActualizado = JsonSerializer.Deserialize<Producto>(await response.Content.ReadAsStringAsync(), jsonOpt);
            Console.WriteLine($"Stock agregado. Nuevo stock de {productoActualizado?.Nombre}: {productoActualizado?.Stock}");
        }
        else
        {
            Console.WriteLine($"Error al agregar stock: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Error de conexión: {e.Message}");
    }
}

async Task QuitarStockAsync()
{
    Console.Write("\nIngrese el ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    Console.Write("Ingrese la cantidad a quitar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida.");
        return;
    }

    try
    {
        var response = await http.PostAsync($"{baseUrl}/productos/quitarstock/{id}/{cantidad}", null);
        if (response.IsSuccessStatusCode)
        {
            var productoActualizado = JsonSerializer.Deserialize<Producto>(await response.Content.ReadAsStringAsync(), jsonOpt);
            Console.WriteLine($"Stock quitado. Nuevo stock de {productoActualizado?.Nombre}: {productoActualizado?.Stock}");
        }
        else
        {
            Console.WriteLine($"Error al quitar stock: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"Error de conexión: {e.Message}");
    }
}

while (true)
{
    Console.WriteLine("\n--- Menú Principal ---");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("5. Salir");
    Console.Write("Seleccione una opción: ");

    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductosAsync();
            break;
        case "2":
            await ListarProductosAReponerAsync();
            break;
        case "3":
            await AgregarStockAsync();
            break;
        case "4":
            await QuitarStockAsync();
            break;
        case "5":
            Console.WriteLine("Saliendo de la aplicación.");
            return;
        default:
            Console.WriteLine("Opción no válida. Intente de nuevo.");
            break;
    }
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
}