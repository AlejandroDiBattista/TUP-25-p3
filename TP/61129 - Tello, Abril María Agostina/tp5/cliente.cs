using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

Console.WriteLine("Aplicacion cliente - Gestion de productos");

 while (true) {
    Console.WriteLine("Seleccione una opcion:");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos para reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("5. Salir");
    Console.Write("Opcion: ");
    var opcion = Console.ReadLine();

    if (opcion == "1") {
        await ListarProductosAsync();
    }else if (opcion == "2") {
        await ListarProductosParaReponerAsync();
    }else if (opcion == "3") {
        await AgregarStockAsync(true);
    }
    else if (opcion == "4") {
        await AgregarStockAsync(false);
    }else if (opcion == "5") {
        break;
    }else {
        Console.WriteLine("Opcion invalida. Intente nuevamente.");
    }
 }

 async Task ListarProductosAsync() 
    {
        try {
             var json = await http.GetStringAsync($"{baseUrl}/productos");
             var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);
             if(productos == null){
                    Console.WriteLine("No se encontraron productos.");
                    return;
             }
             Console.WriteLine("\nID | Nombre               | Stock");
             Console.WriteLine("---+----------------------+-------");
             foreach (var p in productos)
             Console.WriteLine($"{p.Id,2} | {p.Nombre,-20} | {p.Stock,5}");
            }
        catch (Exception ex) {
         Console.WriteLine("Error al listar productos: " + ex.Message);
        }
    }

    async Task ListarProductosParaReponerAsync() {
        try{
            var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
            var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);

            if (productos.Count == 0)
            {
                Console.WriteLine("No hay productos que deban reponerse.");
                return;
            }
            Console.WriteLine("\nID | Nombre               | Stock");
            Console.WriteLine("---+----------------------+-------");
            foreach (var p in productos){
                Console.WriteLine($"{p.Id,2} | {p.Nombre,-20} | {p.Stock,5}");
            }
        }
        catch (Exception ex) {
            Console.WriteLine("Error al listar productos para reponer: " + ex.Message);
        }
    }

async Task AgregarStockAsync(bool agregar) {
    try{
        Console.Write("Ingrese el ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) {
            Console.WriteLine("ID invalido.");
            return;
        }
        Console.Write("Ingrese la cantidad a agregar: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
        {
            Console.WriteLine("Cantidad inválida.");
            return;
        }
        string accion = agregar ? "agregar" : "quitar";

        var response = await http.PostAsync($"{baseUrl}/productos/{id}/{accion}/{cantidad}", null);

        if (response.IsSuccessStatusCode) {
            Console.WriteLine($"Stock {accion}do correctamente.");
        }
        else {
            Console.WriteLine($"Error: no se pudo {accion} stock. Verifique que el producto exista y que haya stock suficiente para quitar.");
        }
    }catch (Exception ex) {
        Console.WriteLine("Error al modificar stock: " + ex.Message);
    }
}
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

// Fin del ejemplo