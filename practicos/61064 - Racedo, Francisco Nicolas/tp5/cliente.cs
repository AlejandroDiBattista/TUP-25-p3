
using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Collections.Generic; // List
using System.Text;                // Encoding

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

bool salir = false;

while (!salir)
{
    Console.Clear();
    Console.WriteLine("=== Sistema de Gestión de Stock ===");
    Console.WriteLine("1. Ver todos los productos");
    Console.WriteLine("2. Ver productos que necesitan reposición (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("\nSeleccione una opción: ");
    
    string opcion = Console.ReadLine(); // Quitamos el operador ? para evitar el error de nullability
    
    switch (opcion)
    {
        case "1":
            await MostrarTodosLosProductos();
            break;
        case "2":
            await MostrarProductosAReponer();
            break;
        case "3":
            await AgregarStockAProducto();
            break;
        case "4":
            await QuitarStockDeProducto();
            break;
        case "0":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

// Método para mostrar todos los productos
async Task MostrarTodosLosProductos()
{
    Console.Clear();
    Console.WriteLine("=== Listado de Productos ===");
    var productos = await TraerAsync();
    MostrarProductos(productos);
    
    Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
    Console.ReadKey();
}

// Método para mostrar productos con stock bajo
async Task MostrarProductosAReponer()
{
    Console.Clear();
    Console.WriteLine("=== Productos que necesitan reposición (stock < 3) ===");
    var productos = await TraerAReponerAsync();
    MostrarProductos(productos);
    
    Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
    Console.ReadKey();
}

// Método para agregar stock a un producto
async Task AgregarStockAProducto()
{
    Console.Clear();
    Console.WriteLine("=== Agregar Stock a Producto ===");
    
    int id = SolicitarIdProducto();
    if (id <= 0) return;
    
    Console.Write("Cantidad a agregar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad no válida. Debe ser un número entero positivo.");
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey();
        return;
    }
    
    try
    {
        var producto = await ModificarStockAsync(id, cantidad, "agregar");
        Console.WriteLine($"\nStock actualizado correctamente para '{producto.Nombre}'");
        Console.WriteLine($"Stock actual: {producto.Stock}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    
    Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
    Console.ReadKey();
}

// Método para quitar stock a un producto
async Task QuitarStockDeProducto()
{
    Console.Clear();
    Console.WriteLine("=== Quitar Stock a Producto ===");
    
    int id = SolicitarIdProducto();
    if (id <= 0) return;
    
    Console.Write("Cantidad a quitar: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad no válida. Debe ser un número entero positivo.");
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey();
        return;
    }
    
    try
    {
        var producto = await ModificarStockAsync(id, cantidad, "quitar");
        Console.WriteLine($"\nStock actualizado correctamente para '{producto.Nombre}'");
        Console.WriteLine($"Stock actual: {producto.Stock}");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Error: No hay suficiente stock disponible o el producto no existe. ({ex.Message})"); // Usamos ex para evitar el warning
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    
    Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
    Console.ReadKey();
}

// Método auxiliar para solicitar ID de producto
int SolicitarIdProducto()
{
    Console.Write("ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
    {
        Console.WriteLine("ID no válido. Debe ser un número entero positivo.");
        Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
        Console.ReadKey();
        return -1;
    }
    return id;
}

// Método auxiliar para mostrar productos en formato de tabla
void MostrarProductos(List<Producto> productos)
{
    if (productos.Count == 0)
    {
        Console.WriteLine("No hay productos disponibles.");
        return;
    }
    
    // Aumentamos el ancho de la tabla para acomodar precios más grandes
    Console.WriteLine(new string('-', 80));
    Console.WriteLine($"{"ID",-5} {"Nombre",-20} {"Precio",20} {"Stock",10}");
    Console.WriteLine(new string('-', 80));
    
    foreach (var p in productos)
    {
        // Ajustamos el ancho de la columna Precio a 20 para números grandes
        Console.WriteLine($"{p.Id,-5} {p.Nombre,-20} {p.Precio,20:c} {p.Stock,10}");
    }
    
    Console.WriteLine(new string('-', 80));
}

// Métodos para comunicarse con el servidor
async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerAReponerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<Producto> ModificarStockAsync(int id, int cantidad, string accion)
{
    var content = new StringContent(
        JsonSerializer.Serialize(new { Cantidad = cantidad }, jsonOpt),
        Encoding.UTF8,
        "application/json");
        
    var response = await http.PutAsync($"{baseUrl}/productos/{id}/{accion}", content);
    response.EnsureSuccessStatusCode();
    
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt)!;
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}