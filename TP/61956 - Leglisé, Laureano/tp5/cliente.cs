using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();

async Task<List<Producto>> ObtenerProductos() =>
    await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos") ?? new();

async Task<List<Producto>> ObtenerProductosAReponer() =>
    await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/reponer") ?? new();

async Task<bool> ActualizarStock(int id, int cantidad, bool agregar)
{
    var url = $"{baseUrl}/productos/{id}/" + (agregar ? "agregar" : "quitar");
    var response = await http.PutAsJsonAsync(url, new { Cantidad = cantidad });
    return response.IsSuccessStatusCode;
}

void MostrarProductos(List<Producto> productos)
{
    Console.WriteLine("ID  Nombre              Precio    Stock");
    Console.WriteLine("-------------------------------------------------");
    foreach(var p in productos)
    {
        Console.WriteLine($"{p.Id,-3} {p.Nombre,-18} {p.Precio,7:C} {p.Stock,6}");
    }
}

async Task MenuAsync()
{
    while(true)
    {
        Console.WriteLine("\nOpciones:");
        Console.WriteLine("1 - Listar todos los productos");
        Console.WriteLine("2 - Listar productos a reponer (stock < 3)");
        Console.WriteLine("3 - Agregar stock a un producto");
        Console.WriteLine("4 - Quitar stock a un producto");
        Console.WriteLine("0 - Salir");
        Console.Write("Seleccione una opción: ");

        var opcion = Console.ReadLine();

        switch(opcion)
        {
            case "1":
                var todos = await ObtenerProductos();
                MostrarProductos(todos);
                break;
            case "2":
                var aReponer = await ObtenerProductosAReponer();
                MostrarProductos(aReponer);
                break;
            case "3":
                Console.Write("ID del producto para agregar stock: ");
                if (!int.TryParse(Console.ReadLine(), out int idAgregar))
                {
                    Console.WriteLine("ID inválido.");
                    break;
                }
                Console.Write("Cantidad a agregar: ");
                if (!int.TryParse(Console.ReadLine(), out int cantAgregar) || cantAgregar <= 0)
                {
                    Console.WriteLine("Cantidad inválida.");
                    break;
                }
                var exitoAgregar = await ActualizarStock(idAgregar, cantAgregar, true);
                Console.WriteLine(exitoAgregar ? "Stock agregado con éxito." : "Error al agregar stock.");
                break;
            case "4":
                Console.Write("ID del producto para quitar stock: ");
                if (!int.TryParse(Console.ReadLine(), out int idQuitar))
                {
                    Console.WriteLine("ID inválido.");
                    break;
                }
                Console.Write("Cantidad a quitar: ");
                if (!int.TryParse(Console.ReadLine(), out int cantQuitar) || cantQuitar <= 0)
                {
                    Console.WriteLine("Cantidad inválida.");
                    break;
                }
                var exitoQuitar = await ActualizarStock(idQuitar, cantQuitar, false);
                Console.WriteLine(exitoQuitar ? "Stock quitado con éxito." : "Error al quitar stock (posible stock insuficiente).");
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
}

await MenuAsync();

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
