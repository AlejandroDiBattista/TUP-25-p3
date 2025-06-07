// Archivo: cliente.cs
// Versión moderna con "Top-Level Statements" (.NET 6+)

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // Requiere el paquete NuGet System.Net.Http.Json
using System.Text.Json;
using System.Threading.Tasks;

// --- MODELO DE DATOS (Debe coincidir con el del servidor) ---
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// --- CONFIGURACIÓN Y LÓGICA PRINCIPAL ---
var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

bool salir = false;
while (!salir)
{
    Console.Clear();
    Console.WriteLine("======================================");
    Console.WriteLine("  Gestión de Stock - Tienda Deportiva");
    Console.WriteLine("======================================");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos para reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock de un producto");
    Console.WriteLine("5. Salir");
    Console.WriteLine("--------------------------------------");
    Console.Write("Seleccione una opción: ");

    switch (Console.ReadLine())
    {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarProductosParaReponer();
            break;
        case "3":
            await AgregarStock();
            break;
        case "4":
            await QuitarStock();
            break;
        case "5":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción no válida. Intente de nuevo.");
            break;
    }

    if (!salir)
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}
Console.WriteLine("¡Hasta luego!");


// --- FUNCIONES AUXILIARES ---

void ImprimirProductos(IEnumerable<Producto> productos, string titulo)
{
    Console.WriteLine($"\n--- {titulo} ---");
    Console.WriteLine($"{"ID",-5} {"Nombre",-25} {"Precio",-15} {"Stock"}");
    Console.WriteLine(new string('-', 60));
    foreach (var p in productos)
    {
        // Formato de moneda para Argentina (ARS)
        Console.WriteLine($"{p.Id,-5} {p.Nombre,-25} {p.Precio.ToString("C", new System.Globalization.CultureInfo("es-AR")),-15} {p.Stock,5}");
    }
    Console.WriteLine(new string('-', 60));
}

async Task ListarProductos()
{
    try
    {
        var productos = await httpClient.GetFromJsonAsync<List<Producto>>("productos", jsonOptions);
        if (productos != null && productos.Any())
        {
            ImprimirProductos(productos, "Listado de Todos los Productos");
        }
        else
        {
            Console.WriteLine("\nNo se encontraron productos.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError al conectar con el servidor: {ex.Message}");
    }
}

async Task ListarProductosParaReponer()
{
    try
    {
        var productos = await httpClient.GetFromJsonAsync<List<Producto>>("productos/reponer", jsonOptions);
        if (productos != null && productos.Any())
        {
            ImprimirProductos(productos, "Productos con Bajo Stock (Reponer)");
        }
        else
        {
            Console.WriteLine("\n¡Excelente! No hay productos que necesiten reposición.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError al conectar con el servidor: {ex.Message}");
    }
}

async Task AgregarStock()
{
    Console.Write("\nIngrese el ID del producto para agregar stock: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido.");
        return;
    }

    Console.Write("Ingrese la cantidad a AGREGAR: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad no válida. Debe ser un número positivo.");
        return;
    }

    try
    {
        var response = await httpClient.PutAsync($"productos/{id}/agregar/{cantidad}", null);
        if (response.IsSuccessStatusCode)
        {
            var productoActualizado = await response.Content.ReadFromJsonAsync<Producto>(jsonOptions);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n¡Éxito! Nuevo stock para '{productoActualizado?.Nombre}': {productoActualizado?.Stock}");
            Console.ResetColor();
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<object>();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {error}");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError al conectar con el servidor: {ex.Message}");
    }
}

async Task QuitarStock()
{
    Console.Write("\nIngrese el ID del producto para quitar stock: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido.");
        return;
    }

    Console.Write("Ingrese la cantidad a QUITAR: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad no válida. Debe ser un número positivo.");
        return;
    }

    try
    {
        var response = await httpClient.PutAsync($"productos/{id}/quitar/{cantidad}", null);
        if (response.IsSuccessStatusCode)
        {
            var productoActualizado = await response.Content.ReadFromJsonAsync<Producto>(jsonOptions);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n¡Éxito! Nuevo stock para '{productoActualizado?.Nombre}': {productoActualizado?.Stock}");
            Console.ResetColor();
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<object>();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {error}");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError al conectar con el servidor: {ex.Message}");
    }
}