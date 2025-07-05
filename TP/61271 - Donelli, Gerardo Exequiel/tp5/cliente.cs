using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Clase Producto con stock
class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// Traer todos los productos
async Task<List<Producto>> TraerProductosAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

// Traer productos con stock < 3
async Task<List<Producto>> TraerProductosPocoStockAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/stock");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

// Agregar stock a producto
async Task<bool> AgregarStockAsync(int id, int cantidad)
{
    var response = await http.PutAsync($"{baseUrl}/productos/agregar/{id}/{cantidad}", null);
    return response.IsSuccessStatusCode;
}

// Quitar stock de producto
async Task<bool> QuitarStockAsync(int id, int cantidad)
{
    var response = await http.PutAsync($"{baseUrl}/productos/quitar/{id}/{cantidad}", null);
    return response.IsSuccessStatusCode;
}

// Leer entero con validación
int LeerEntero(string mensaje)
{
    int val;
    do
    {
        Console.Write(mensaje);
    } while (!int.TryParse(Console.ReadLine(), out val));
    return val;
}

// Menú principal
int opcion;
do
{
    Console.Clear();
    Console.WriteLine("=== MENÚ ===");
    Console.WriteLine("1 - Listar todos los productos");
    Console.WriteLine("2 - Listar productos con stock menor a 3");
    Console.WriteLine("3 - Agregar stock a un producto");
    Console.WriteLine("4 - Quitar stock a un producto");
    Console.WriteLine("0 - Salir");
    opcion = LeerEntero("Elija opción: ");

    switch (opcion)
    {
        case 1:
            var todos = await TraerProductosAsync();
            Console.WriteLine("\nProductos disponibles:");
            foreach (var p in todos)
            {
                Console.WriteLine($"{p.Id} - {p.Nombre} - Precio: {p.Precio:C} - Stock: {p.Stock}");
            }
            break;

        case 2:
            var pocoStock = await TraerProductosPocoStockAsync();
            Console.WriteLine("\nProductos con stock menor a 3:");
            foreach (var p in pocoStock)
            {
                Console.WriteLine($"{p.Id} - {p.Nombre} - Precio: {p.Precio:C} - Stock: {p.Stock}");
            }
            break;

        case 3:
            int idAgregar = LeerEntero("ID producto para agregar stock: ");
            int cantAgregar = LeerEntero("Cantidad a agregar: ");
            if (await AgregarStockAsync(idAgregar, cantAgregar))
                Console.WriteLine("Stock agregado correctamente.");
            else
                Console.WriteLine("Error al agregar stock.");
            break;

        case 4:
            int idQuitar = LeerEntero("ID producto para quitar stock: ");
            int cantQuitar = LeerEntero("Cantidad a quitar: ");
            if (await QuitarStockAsync(idQuitar, cantQuitar))
                Console.WriteLine("Stock quitado correctamente.");
            else
                Console.WriteLine("Error al quitar stock (¿stock insuficiente?).");
            break;

        case 0:
            Console.WriteLine("Saliendo...");
            break;

        default:
            Console.WriteLine("Opción inválida.");
            break;
    }

    if (opcion != 0)
    {
        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }
} while (opcion != 0);
// Fin del programa
//     };