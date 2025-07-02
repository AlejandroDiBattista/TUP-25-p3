using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace cliente
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public int Stock { get; set; }
    }

    class Program
    {
        static readonly HttpClient client = new() { BaseAddress = new Uri("http://localhost:5000") };

        static async Task ListarTodos()
        {
            var lista = await client.GetFromJsonAsync<List<Producto>>("productos");
            Console.WriteLine("\n-- Todos los Productos --");
            if (lista is null) { Console.WriteLine("Error al obtener lista."); return; }
            lista.ForEach(p => Console.WriteLine($"{p.Id}: {p.Nombre} (Stock: {p.Stock})"));
        }

        static async Task ListarBajoStock()
        {
            var lista = await client.GetFromJsonAsync<List<Producto>>("productos/bajo-stock");
            Console.WriteLine("\n-- Productos Bajo Stock --");
            if (lista is null) { Console.WriteLine("Error al obtener lista."); return; }
            lista.ForEach(p => Console.WriteLine($"{p.Id}: {p.Nombre} (Stock: {p.Stock})"));
        }

        static async Task CambiarStock(bool agregar)
        {
            Console.Write("ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int cant)) return;

            string ruta = agregar
                ? $"productos/{id}/agregar?cantidad={cant}"
                : $"productos/{id}/quitar?cantidad={cant}";

            var resp = await client.PostAsync(ruta, null);
            var contenido = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
                Console.WriteLine($"OK: {contenido}");
            else
                Console.WriteLine($"Error ({(int)resp.StatusCode}): {contenido}");
        }

        static async Task Main()
        {
            while (true)
            {
                Console.WriteLine("\n1) Listar todos");
                Console.WriteLine("2) Listar bajo stock");
                Console.WriteLine("3) Agregar stock");
                Console.WriteLine("4) Quitar stock");
                Console.WriteLine("0) Salir");
                Console.Write("Opción: ");
                var op = Console.ReadLine();

                if (op == "0") break;
                switch (op)
                {
                    case "1": await ListarTodos(); break;
                    case "2": await ListarBajoStock(); break;
                    case "3": await CambiarStock(true); break;
                    case "4": await CambiarStock(false); break;
                    default: Console.WriteLine("Opción inválida."); break;
                }
            }
        }
    }
}

