using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Text;                // Encoding.UTF8

using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Text;                // Encoding.UTF8

class Program
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://localhost:5000";
        var http = new HttpClient();
        var jsonOpt = new JsonSerializerOptions {
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        // Codigo de ejemplo: Reemplazar por la implementacion real 
        async Task<List<Producto>> TraerAsync(){
            var json = await http.GetStringAsync($"{baseUrl}/productos");
            return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
        }

        Console.WriteLine("=== Productos ===");
        foreach (var p in await TraerAsync()) {
            Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c}");
        }

        // ===== A partir de acá, implementación real =====

        bool salir = false;
        while (!salir)
        {
            Console.WriteLine("\n=== MENÚ ===");
            Console.WriteLine("1. Listar productos");
            Console.WriteLine("2. Ver productos con bajo stock");
            Console.WriteLine("3. Agregar stock");
            Console.WriteLine("4. Quitar stock");
            Console.WriteLine("0. Salir");
            Console.Write("Elegí una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await ListarProductos();
                    break;
                case "2":
                    await VerBajoStock();
                    break;
                case "3":
                    await ModificarStock("sumar");
                    break;
                case "4":
                    await ModificarStock("restar");
                    break;
                case "0":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }

        // === Funciones ===

        async Task ListarProductos()
        {
            var productos = await TraerAsync();
            Console.WriteLine("\nID | Nombre                 | Precio    | Stock");
            foreach (var p in productos)
                Console.WriteLine($"{p.Id,2} | {p.Nombre,-20} | {p.Precio,8:c} | {p.Stock}");
        }

        async Task VerBajoStock()
        {
            var json = await http.GetStringAsync($"{baseUrl}/productos/bajo-stock");
            var lista = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
            Console.WriteLine("\nProductos con bajo stock (<3):");
            foreach (var p in lista)
                Console.WriteLine($"{p.Id}. {p.Nombre} - Stock: {p.Stock}");
        }

        async Task ModificarStock(string accion)
        {
            Console.Write("Ingrese el ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            Console.Write($"Ingrese la cantidad a {accion}: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad))
            {
                Console.WriteLine("Cantidad inválida.");
                return;
            }

            var url = $"{baseUrl}/productos/{id}/{accion}?cantidad={cantidad}";
            var response = await http.PutAsync(url, new StringContent("", Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Stock actualizado correctamente.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
            }
        }
    }
}

// Clase del modelo
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; } // Agregado para ver stock
}
