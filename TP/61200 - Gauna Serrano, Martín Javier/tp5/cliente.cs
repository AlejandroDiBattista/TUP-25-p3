#r "nuget: System.Net.Http.Json"

using System.Net.Http;
using System.Net.Http.Json;

// Base URL del servidor
var baseUrl = "http://localhost:5000";

var http = new HttpClient();

while (true)
{
    Console.Clear();
    Console.WriteLine("==== Menú Tienda ====");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Ver productos que necesitan reposición (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");

    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductos();
            break;
        case "2":
            await ListarProductosConPocoStock();
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
            Console.WriteLine("Opción no válida.");
            break;
    }

    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

// ----------------- FUNCIONES -----------------

async Task ListarProductos()
{
    var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos");
    Console.WriteLine("\n--- Productos disponibles ---");
    foreach (var p in productos)
    {
        Console.WriteLine($"{p.Id}: {p.Nombre} - Stock: {p.Stock}");
    }
}

async Task ListarProductosConPocoStock()
{
    var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/reponer");
    Console.WriteLine("\n--- Productos a reponer ---");
    foreach (var p in productos)
    {
        Console.WriteLine($"{p.Id}: {p.Nombre} - Stock: {p.Stock}");
    }
}

async Task ModificarStock(string accion)
{
    Console.Write("Ingrese ID del producto: ");
    var id = Console.ReadLine();
    Console.Write("Ingrese cantidad: ");
    var cantidad = Console.ReadLine();

    var url = $"{baseUrl}/productos/{id}/{accion}?cantidad={cantidad}";
    var respuesta = await http.PostAsync(url, null);

    if (respuesta.IsSuccessStatusCode)
    {
        var producto = await respuesta.Content.ReadFromJsonAsync<Producto>();
        Console.WriteLine($"\n✅ Producto actualizado: {producto.Nombre} - Nuevo stock: {producto.Stock}");
    }
    else
    {
        var error = await respuesta.Content.ReadAsStringAsync();
        Console.WriteLine($"\n❌ Error: {error}");
    }
}

// -------------- MODELO -----------------

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}