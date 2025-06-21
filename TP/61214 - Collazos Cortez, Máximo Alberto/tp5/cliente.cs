#r "nuget: System.Net.Http.Json"

using System.Net.Http.Json;
public class Producto_cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
}

HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000");

while (true)
{
    Console.WriteLine("\n--- MENÚ ---");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Elija una opción: ");
    string? opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            var productos = await client.GetFromJsonAsync<List<Producto_cliente>>("/productos");
            foreach (var p in productos!) Console.WriteLine($"{p.Id} - {p.Nombre} (Stock: {p.Stock})");
            break;

        case "2":
            var reponer = await client.GetFromJsonAsync<List<Producto_cliente>>("/productos/reponer");
            foreach (var p in reponer!) Console.WriteLine($"{p.Id} - {p.Nombre} (Stock: {p.Stock})");
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idAdd = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantAdd = int.Parse(Console.ReadLine()!);
            var resAdd = await client.PostAsync($"/productos/agregar/{idAdd}/{cantAdd}", null);
            Console.WriteLine(resAdd.IsSuccessStatusCode ? "Stock actualizado." : "Error al agregar stock.");
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idRemove = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantRemove = int.Parse(Console.ReadLine()!);
            var response = await client.PostAsync($"/productos/quitar/{idRemove}/{cantRemove}", null);
            Console.WriteLine(response.IsSuccessStatusCode ? "Stock actualizado." : "Error: No se pudo quitar.");
            break;

        case "0":
            return;
    }
}
