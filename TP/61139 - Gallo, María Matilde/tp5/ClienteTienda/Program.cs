

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true) {
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Ver productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    switch (opcion) {
        case "1":
            var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos");
            Mostrar(productos!);
            break;

        case "2":
            var reponer = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/reponer");
            Mostrar(reponer!);
            break;

        case "3":
            Console.Write("ID del producto: ");
            int idAdd = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantAdd = int.Parse(Console.ReadLine()!);
            var resAdd = await http.PostAsync($"{baseUrl}/productos/{idAdd}/agregar/{cantAdd}", null);
            Console.WriteLine(resAdd.IsSuccessStatusCode ? "✔️ Stock agregado." : "❌ Error.");
            break;

        case "4":
            Console.Write("ID del producto: ");
            int idRem = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantRem = int.Parse(Console.ReadLine()!);
            var resRem = await http.PostAsync($"{baseUrl}/productos/{idRem}/quitar/{cantRem}", null);
            Console.WriteLine(resRem.IsSuccessStatusCode ? "✔️ Stock quitado." : "❌ Error.");
            break;

        case "0":
            return;
    }
}

void Mostrar(List<Producto> productos) {
    Console.WriteLine("\nID\tNombre\t\tPrecio\t\tStock");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id}\t{p.Nombre,-15}\t{p.Precio,8:C}\t{p.Stock}");
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

