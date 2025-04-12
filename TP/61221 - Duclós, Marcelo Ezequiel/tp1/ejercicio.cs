using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)


internal class Program
{
    private static void Main(string[] args)
    {
        static void MostrarMenu()
        {
            Console.WriteLine("===== AGENDA DE CONTACTOS =====");
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contactos");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("0) Salir");
            Console.Write("Seleccione una opción: ");
        }

        static void AgregarContacto()
        {
            if (cantidad >= MAX_CONTACTOS)
            {
                Console.WriteLine("No se pueden agregar más contactos. Límite alcanzado.");
                return;
            }

            Console.WriteLine("=== Agregar Contacto ===");
            Console.Write("Nombre   : ");
            string nombre = Console.ReadLine() ?? string.Empty;
            Console.Write("Teléfono : ");
            string telefono = Console.ReadLine() ?? string.Empty;
            Console.Write("Email    : ");
            string email = Console.ReadLine() ?? string.Empty;

            contactos[cantidad].Id = nextId;
            contactos[cantidad].Nombre = nombre;
            contactos[cantidad].Telefono = telefono;
            contactos[cantidad].Email = email;
            cantidad++;
            Console.WriteLine("Contacto agregado con ID = " + nextId);
            nextId++;
        }

        static void ModificarContacto()
        {
            Console.WriteLine("=== Modificar Contacto ===");
            Console.Write("Ingrese el ID del contacto a modificar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID no válido.");
                return;
            }

            int index = BuscarContactoPorId(id);
            if (index == -1)
            {
                Console.WriteLine("Contacto con ID=" + id + " no encontrado.");
                return;
            }

            contactos[index].Nombre = SolicitarNuevoValor(contactos[index].Nombre, "Nombre");
            contactos[index].Telefono = SolicitarNuevoValor(contactos[index].Telefono, "Teléfono");
            contactos[index].Email = SolicitarNuevoValor(contactos[index].Email, "Email");

            Console.WriteLine("Contacto modificado con éxito.");
        }

        static void BorrarContacto()
        {
            Console.WriteLine("=== Borrar Contacto ===");
            Console.Write("Ingrese el ID del contacto a borrar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID no válido.");
                return;
            }

            int index = BuscarContactoPorId(id);
            if (index == -1)
            {
                Console.WriteLine("Contacto con ID=" + id + " no encontrado.");
                return;
            }

            for (int i = index; i < cantidad - 1; i++)
            {
                contactos[i] = contactos[i + 1];
            }
            cantidad--;
            Console.WriteLine("Contacto con ID=" + id + " eliminado con éxito.");
        }

        static void ListarContactos()
        {
            Console.WriteLine("=== Lista de Contactos ===");
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}",
                    contactos[i].Id,
                    contactos[i].Nombre,
                    contactos[i].Telefono,
                    contactos[i].Email);
            }
        }

        static void BuscarContacto()
        {
            Console.WriteLine("=== Buscar Contacto ===");
            Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
            string termino = Console.ReadLine() ?? string.Empty;
            if (termino == null) termino = string.Empty;
            termino = termino.ToLower();

            Console.WriteLine("\nResultados de la búsqueda:");
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");

            for (int i = 0; i < cantidad; i++)
            {
                if (contactos[i].Nombre.ToLower().Contains(termino) ||
                    contactos[i].Telefono.ToLower().Contains(termino) ||
                    contactos[i].Email.ToLower().Contains(termino))
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}",
                        contactos[i].Id,
                        contactos[i].Nombre,
                        contactos[i].Telefono,
                        contactos[i].Email);
                }
            }
        }

        static void CargarContactos()
        {
            if (!File.Exists("agenda.csv")) return;

            string[] lineas = File.ReadAllLines("agenda.csv");
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split(',');
                if (partes.Length < 4 || !int.TryParse(partes[0], out int id)) continue;

                contactos[cantidad].Id = id;
                contactos[cantidad].Nombre = partes[1];
                contactos[cantidad].Telefono = partes[2];
                contactos[cantidad].Email = partes[3];
                cantidad++;
                if (id >= nextId) nextId = id + 1;
            }
        }

        static void GuardarContactos()
        {
            string[] lineas = new string[cantidad];
            for (int i = 0; i < cantidad; i++)
            {
                lineas[i] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
            }
            File.WriteAllLines("agenda.csv", lineas);
        }

        static int BuscarContactoPorId(int id)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (contactos[i].Id == id)
                    return i;
            }
            return -1;
        }

        static string SolicitarNuevoValor(string actual, string campo)
        {
            Console.Write($"{campo} (Actual: {actual}): ");
            string nuevo = Console.ReadLine() ?? string.Empty;
            return string.IsNullOrWhiteSpace(nuevo) ? actual : nuevo;
        }
    }
}