using System; // Console, etc.
using System.Linq; // OrderBy
using System.Net.Http; // HttpClient, StringContent ✔
using System.Text.Json; // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks; // Task

async Task MostrarMenuAsync()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("\n=== Menú Inventario ===");
        Console.WriteLine("1. Listar productos");
        Console.WriteLine("2. Ver producto por ID");
        Console.WriteLine("3. Listar productos con stock crítico");
        Console.WriteLine("4. Agregar producto");
        Console.WriteLine("5. Modificar producto");
        Console.WriteLine("6. Modificar stock de producto");
        Console.WriteLine("7. Eliminar producto");
        Console.WriteLine("0. Salir");
        Console.Write("\nSeleccione una opción: ");
        var opcion = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(opcion))
        {
            Console.WriteLine("Opción inválida.");
            continue;
        }
        Console.Clear();
        switch (opcion)
        {
            case "1":
                await ListarProductosAsync();
                EsperarTecla();
                break;
            case "2":
                await VerProductoPorIdAsync();
                EsperarTecla();
                break;
            case "3":
                await ListarStockCriticoAsync();
                EsperarTecla();
                break;
            case "4":
                await AgregarProductoAsync();
                EsperarTecla();
                break;
            case "5":
                await ModificarProductoAsync();
                EsperarTecla();
                break;
            case "6":
                await ModificarStockAsync();
                EsperarTecla();
                break;
            case "7":
                await EliminarProductoAsync();
                EsperarTecla();
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
}

void EsperarTecla()
{
    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

async Task ListarProductosAsync()
{
    var productos = await Inventario.TraerAsync();
    Console.WriteLine("\nID  Nombre                Precio           Stock");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id, -3} {p.Nombre, -20} {p.Precio, 10:c} {p.Stock, 10}");
}

async Task VerProductoPorIdAsync()
{
    Console.Write("Ingrese ID de producto: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        try
        {
            var p = await Inventario.TraerAsync(id);
            Console.WriteLine(
                $"ID: {p.Id}\nNombre: {p.Nombre}\nPrecio: {p.Precio:c}\nStock: {p.Stock}"
            );
        }
        catch
        {
            Console.WriteLine("Producto no encontrado.");
        }
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
}

async Task ListarStockCriticoAsync()
{
    var productos = await Inventario.TraerAsync(true);
    Console.WriteLine("\nProductos con stock crítico:");
    Console.WriteLine("\nID  Nombre                Precio           Stock");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id, -3} {p.Nombre, -20} {p.Precio, 10:c} {p.Stock, 10}");
}

async Task AgregarProductoAsync()
{
    Console.Write("Nombre: ");
    var nombre = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(nombre))
    {
        nombre = "Producto sin nombre";
    }
    Console.Write("Precio: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal precio) || precio < 0)
    {
        Console.WriteLine("Precio inválido.");
        return;
    }
    Console.Write("Stock: ");
    if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
    {
        Console.WriteLine("Stock inválido.");
        return;
    }
    var nuevo = new Producto
    {
        Nombre = nombre,
        Precio = precio,
        Stock = stock,
    };
    var agregado = await Inventario.AgregarAsync(nuevo);
    Console.WriteLine($"Producto agregado con ID {agregado.Id}");
}

async Task ModificarProductoAsync()
{
    Console.Write("ID del producto a modificar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }
    try
    {
        var p = await Inventario.TraerAsync(id);
        Console.WriteLine("* Deje vacío para no modificar el campo *");
        Console.Write($"Nombre ({p.Nombre}): ");
        var nombre = Console.ReadLine();
        Console.Write($"Precio ({p.Precio}): ");
        var precioStr = Console.ReadLine();

        if (
            !string.IsNullOrWhiteSpace(precioStr)
            && (!decimal.TryParse(precioStr, out decimal precio) || precio < 0)
        )
        {
            Console.WriteLine("Precio inválido.");
            return;
        }
        Console.Write($"Stock ({p.Stock}): ");
        var stockStr = Console.ReadLine();
        if (
            !string.IsNullOrWhiteSpace(stockStr)
            && (!int.TryParse(stockStr, out int stock) || stock < 0)
        )
        {
            Console.WriteLine("Stock inválido.");
            return;
        }

        p.Nombre = string.IsNullOrWhiteSpace(nombre) ? p.Nombre : nombre;
        p.Precio = string.IsNullOrWhiteSpace(precioStr) ? p.Precio : decimal.Parse(precioStr);
        p.Stock = string.IsNullOrWhiteSpace(stockStr) ? p.Stock : int.Parse(stockStr);

        var modificado = await Inventario.ModificarAsync(p);
        Console.WriteLine("Producto modificado.");
        Console.WriteLine(
            $"ID: {modificado.Id}\nNombre: {modificado.Nombre}\nPrecio: {modificado.Precio:c}\nStock: {modificado.Stock}"
        );
    }
    catch
    {
        Console.WriteLine("Producto no encontrado.");
    }
}

async Task ModificarStockAsync()
{
    Console.Write("ID de producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }
    Console.Write("Cantidad en que varía el stock: ");
    if (!int.TryParse(Console.ReadLine(), out int stock))
    {
        Console.WriteLine("Stock inválido.");
        return;
    }
    var ok = await Inventario.ModificarStockAsync(id, stock);
    if (ok)
    {
        var p = await Inventario.TraerAsync(id);
        Console.WriteLine("Stock actualizado.");
        Console.WriteLine(
            $"ID: {p.Id}\nNombre: {p.Nombre}\nPrecio: {p.Precio:c}\nStock: {p.Stock}"
        );
    }
    else
    {
        Console.WriteLine("No se pudo actualizar el stock.");
    }
}

async Task EliminarProductoAsync()
{
    Console.Write("ID a eliminar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }
    var ok = await Inventario.EliminarAsync(id);
    Console.WriteLine(ok ? "Producto eliminado." : "No se pudo eliminar el producto.");
}

await MostrarMenuAsync();

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

record StockDto(int Stock); // DTO para actualizar el stock

static class Inventario
{
    private static readonly string baseUrl = "http://localhost:5000";
    private static readonly HttpClient http = new HttpClient();
    private static readonly JsonSerializerOptions jsonOpt = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public static async Task<List<Producto>> TraerAsync()
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }

    public static async Task<Producto> TraerAsync(int id)
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos/{id}");
        return JsonSerializer.Deserialize<Producto>(json, jsonOpt)!;
    }

    public static async Task<List<Producto>> TraerAsync(bool stockCritico)
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos/stockCritico");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }

    public static async Task<Producto> AgregarAsync(Producto p)
    {
        var json = JsonSerializer.Serialize(p, jsonOpt);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await http.PostAsync($"{baseUrl}/productos", content);
        response.EnsureSuccessStatusCode();
        json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Producto>(json, jsonOpt)!;
    }

    public static async Task<Producto> ModificarAsync(Producto p)
    {
        var json = JsonSerializer.Serialize(p, jsonOpt);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await http.PutAsync($"{baseUrl}/productos/{p.Id}", content);
        response.EnsureSuccessStatusCode();
        json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Producto>(json, jsonOpt)!;
    }

    public static async Task<bool> ModificarStockAsync(int id, int stock)
    {
        var dto = new StockDto(stock);
        var json = JsonSerializer.Serialize(dto, jsonOpt);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await http.PutAsync($"{baseUrl}/productos/{id}/stock", content);
        return response.IsSuccessStatusCode;
    }

    public static async Task<bool> EliminarAsync(int id)
    {
        var response = await http.DeleteAsync($"{baseUrl}/productos/{id}");
        return response.IsSuccessStatusCode;
    }
}
