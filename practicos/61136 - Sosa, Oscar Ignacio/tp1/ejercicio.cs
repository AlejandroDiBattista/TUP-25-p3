using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program
{
    const int MAX_CONTACTOS = 100;
    static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
    static int totalContactos = 0;
    static int ultimoId = 0;
    const string ARCHIVO = "agenda.csv";

    static void Main()
    {
        CargarDesdeArchivo();

        while (true)
        {
            Console.WriteLine("\n--- Menú de Agenda ---");
            Console.WriteLine("a. Agregar contacto");
            Console.WriteLine("b. Modificar contacto");
            Console.WriteLine("c. Borrar contacto");
            Console.WriteLine("d. Listar contactos");
            Console.WriteLine("e. Buscar contacto");
            Console.WriteLine("f. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine().ToLower();

            if (opcion == "a") AgregarContacto();
            else if (opcion == "b") ModificarContacto();
            else if (opcion == "c") BorrarContacto();
            else if (opcion == "d") ListarContactos();
            else if (opcion == "e") BuscarContacto();
            else if (opcion == "f")
            {
                GuardarEnArchivo();
                break;
            }
            else Console.WriteLine("Opción no válida.");
        }
    }

    static void AgregarContacto()
    {
        if (totalContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("La agenda está llena.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        ultimoId++;
        agenda[totalContactos] = new Contacto { Id = ultimoId, Nombre = nombre, Telefono = telefono, Email = email };
        totalContactos++;

        Console.WriteLine("Contacto agregado con éxito.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write($"Nuevo nombre ({agenda[i].Nombre}): ");
                string nombre = Console.ReadLine();
                if (nombre != "") agenda[i].Nombre = nombre;

                Console.Write($"Nuevo teléfono ({agenda[i].Telefono}): ");
                string telefono = Console.ReadLine();
                if (telefono != "") agenda[i].Telefono = telefono;

                Console.Write($"Nuevo email ({agenda[i].Email}): ");
                string email = Console.ReadLine();
                if (email != "") agenda[i].Email = email;

                Console.WriteLine("Contacto modificado.");
                return;
            }
        }

        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a eliminar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }

                totalContactos--;
                Console.WriteLine("Contacto eliminado.");
                return;
            }
        }

        Console.WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Teléfono", "Email");

        for (int i = 0; i < totalContactos; i++)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}",
                agenda[i].Id,
                agenda[i].Nombre,
                agenda[i].Telefono,
                agenda[i].Email);
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("\n{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Teléfono", "Email");

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}",
                    agenda[i].Id,
                    agenda[i].Nombre,
                    agenda[i].Telefono,
                    agenda[i].Email);
            }
        }
    }

    static void CargarDesdeArchivo()
    {
        if (!File.Exists(ARCHIVO)) return;

        string[] lineas = File.ReadAllLines(ARCHIVO);
        for (int i = 0; i < lineas.Length && totalContactos < MAX_CONTACTOS; i++)
        {
            string[] partes = lineas[i].Split(',');

            if (partes.Length == 4)
            {
                int id = int.Parse(partes[0]);
                string nombre = partes[1];
                string telefono = partes[2];
                string email = partes[3];

                agenda[totalContactos] = new Contacto { Id = id, Nombre = nombre, Telefono = telefono, Email = email };
                if (id > ultimoId) ultimoId = id;
                totalContactos++;
            }
        }
    }

    static void GuardarEnArchivo()
    {
        string[] lineas = new string[totalContactos];

        for (int i = 0; i < totalContactos; i++)
        {
            lineas[i] = $"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}";
        }

        File.WriteAllLines(ARCHIVO, lineas);
    }
}
