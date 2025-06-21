using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;


var cliente = new HttpClient();
var opcionesJson = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

var baseUrl = "http://localhost:5000";

async Task<List<Articulo>> ObtenerArticulos() {
    try {
        return await cliente.GetFromJsonAsync<List<Articulo>>($"{baseUrl}/articulos", opcionesJson) ?? new();
    } catch {
        Console.WriteLine("⚠️ Error al conectar con el servidor.");
        return new();
    }
}

async Task<List<Articulo>> ObtenerReposicion() {
    try {
        return await cliente.GetFromJsonAsync<List<Articulo>>($"{baseUrl}/articulos/bajo-stock", opcionesJson) ?? new();
    } catch {
        Console.WriteLine("⚠️ No se pudo recuperar la lista de artículos a reponer.");
        return new();
    }
}

async Task<bool> ModificarStock(int id, int cantidad, bool agregar) {
    try {
        var ruta = agregar ? "incrementar" : "reducir";
        var resp = await cliente.PostAsJsonAsync($"{baseUrl}/articulos/{id}/{ruta}", new { cantidad }, opcionesJson);
        var msj = await resp.Content.ReadAsStringAsync();
        Console.WriteLine(resp.IsSuccessStatusCode ? "✅ Operación exitosa." : $"❌ {msj}");
        return resp.IsSuccessStatusCode;
    } catch (Exception ex) {
        Console.WriteLine($"❌ Error: {ex.Message}");
        return false;
    }
}

async Task MostrarLista(List<Articulo> lista, string titulo) {
    Console.WriteLine($"\n=== {titulo.ToUpper()} ===");
    if (!lista.Any()) {
        Console.WriteLine("Sin resultados.");
        return;
    }
    Console.WriteLine($"{"ID",-4} {"Nombre",-22} {"Precio",-10} {"Stock",-6}");
    Console.WriteLine(new string('-', 48));
    lista.ForEach(a => Console.WriteLine($"{a.Id,-4} {a.Nombre,-22} ${a.Precio,-9:F2} {a.Stock,-6}"));
}

while (true) {
    Console.Clear();
    Console.WriteLine("📦 SISTEMA DE STOCK PARA TIENDA 📷");
    Console.WriteLine("1. Ver artículos");
    Console.WriteLine("2. Ver artículos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("5. Salir");
    Console.Write("Ingrese una opción: ");

    switch (Console.ReadLine()) {
        case "1":
            await MostrarLista(await ObtenerArticulos(), "ARTÍCULOS DISPONIBLES");
            break;

        case "2":
            await MostrarLista(await ObtenerReposicion(), "REPOSICIÓN NECESARIA");
            break;

        case "3":
            Console.Write("Ingrese ID y cantidad a agregar (ej: 2 5): ");
            var entradaA = Console.ReadLine()?.Split(' ');
            if (entradaA?.Length == 2 &&
                int.TryParse(entradaA[0], out int idA) &&
                int.TryParse(entradaA[1], out int cantA) && cantA > 0)
                await ModificarStock(idA, cantA, true);
            else
                Console.WriteLine("❌ Entrada inválida.");
            break;

        case "4":
            Console.Write("Ingrese ID y cantidad a quitar (ej: 3 2): ");
            var entradaQ = Console.ReadLine()?.Split(' ');
            if (entradaQ?.Length == 2 &&
                int.TryParse(entradaQ[0], out int idQ) &&
                int.TryParse(entradaQ[1], out int cantQ) && cantQ > 0)
                await ModificarStock(idQ, cantQ, false);
            else
                Console.WriteLine("❌ Entrada inválida.");
            break;

        case "5":
            Console.WriteLine("Hasta luego 👋");
            return;

        default:
            Console.WriteLine("❌ Opción no válida.");
            break;
    }

    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}
    
class Articulo {
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
