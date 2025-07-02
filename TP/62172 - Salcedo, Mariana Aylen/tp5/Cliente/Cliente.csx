// Cliente para consumir la API REST de stock
// Ejecutar con: dotnet script Cliente.csx
#r "nuget: System.Net.Http.Json, 8.0.0"

using System.Net.Http;
using System.Net.Http.Json;

string baseUrl = "http://localhost:5000";
var http = new HttpClient();

void MostrarMenu()
{
    Console.WriteLine("\n--- Menú ---");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
}

while (true)
{
    MostrarMenu();
    var op = Console.ReadLine();
    if (op == "0") break;
    switch (op)
    {
        case "1":
            var productos = await http.GetFromJsonAsync<List<Producto>>(baseUrl + "/productos");
            Console.WriteLine("\nProductos:");
            foreach (var p in productos!)
                Console.WriteLine($"{p.Id}: {p.Nombre} (Stock: {p.Stock})");
            break;
        case "2":
            var reponer = await http.GetFromJsonAsync<List<Producto>>(baseUrl + "/productos/reponer");
            Console.WriteLine("\nProductos a reponer:");
            foreach (var p in reponer!)
                Console.WriteLine($"{p.Id}: {p.Nombre} (Stock: {p.Stock})");
            break;
        case "3":
            Console.Write("ID del producto: ");
            int idA = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantA = int.Parse(Console.ReadLine()!);
            var respA = await http.PostAsync($"{baseUrl}/productos/{idA}/agregar?cantidad={cantA}", null);
            if (respA.IsSuccessStatusCode)
                Console.WriteLine("Stock agregado correctamente.");
            else
                Console.WriteLine("Error al agregar stock.");
            break;
        case "4":
            Console.Write("ID del producto: ");
            int idQ = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQ = int.Parse(Console.ReadLine()!);
            var respQ = await http.PostAsync($"{baseUrl}/productos/{idQ}/quitar?cantidad={cantQ}", null);
            if (respQ.IsSuccessStatusCode)
                Console.WriteLine("Stock quitado correctamente.");
            else
                Console.WriteLine("Error al quitar stock: " + await respQ.Content.ReadAsStringAsync());
            break;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}
