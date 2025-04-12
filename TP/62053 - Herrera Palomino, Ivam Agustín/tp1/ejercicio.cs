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
    static int contadorContactos = 0;
    static string archivoCSV = "agenda.csv";

    static void Main()
    {
        CargarDesdeArchivo();
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== AGENDA DE CONTACTOS ===");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");
            
            string opcion = Console.ReadLine();
            Console.Clear();
            
            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "6": GuardarEnArchivo(); return;
                default: Console.WriteLine("Opción no válida."); break;
            }
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    static void AgregarContacto()
    {
        if (contadorContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("No se pueden agregar más contactos. Límite alcanzado.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        Contacto nuevo = new Contacto
        {
            Id = contadorContactos + 1,
            Nombre = nombre,
            Telefono = telefono,
            Email = email
        };

        agenda[contadorContactos] = nuevo;
        contadorContactos++;
        Console.WriteLine("Contacto agregado correctamente.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id;
        if (!int.TryParse(Console.ReadLine(), out id) || id <= 0 || id > contadorContactos)
        {
            Console.WriteLine("ID no válido.");
            return;
        }

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write($"Nuevo nombre ({agenda[i].Nombre}): ");
                string nuevoNombre = Console.ReadLine();
                Console.Write($"Nuevo teléfono ({agenda[i].Telefono}): ");
                string nuevoTelefono = Console.ReadLine();
                Console.Write($"Nuevo email ({agenda[i].Email}): ");
                string nuevoEmail = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nuevoNombre)) agenda[i].Nombre = nuevoNombre;
                if (!string.IsNullOrWhiteSpace(nuevoTelefono)) agenda[i].Telefono = nuevoTelefono;
                if (!string.IsNullOrWhiteSpace(nuevoEmail)) agenda[i].Email = nuevoEmail;

                Console.WriteLine("Contacto modificado correctamente.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a eliminar: ");
        int id;
        if (!int.TryParse(Console.ReadLine(), out id) || id <= 0 || id > contadorContactos)
        {
            Console.WriteLine("ID no válido.");
            return;
        }

        int index = -1;
        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Console.WriteLine("Contacto no encontrado.");
            return;
        }

        for (int i = index; i < contadorContactos - 1; i++)
        {
            agenda[i] = agenda[i + 1];
        }

        contadorContactos--;
        Console.WriteLine("Contacto eliminado correctamente.");
    }

    static void ListarContactos()
    {
        if (contadorContactos == 0)
        {
            Console.WriteLine("No hay contactos en la agenda.");
            return;
        }

        Console.WriteLine("ID   Nombre                Teléfono          Email");
        Console.WriteLine("--------------------------------------------------------");

        for (int i = 0; i < contadorContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-4} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese un término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        bool encontrado = false;

        Console.WriteLine("ID   Nombre                Teléfono          Email");
        Console.WriteLine("--------------------------------------------------------");

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-4} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado)
            Console.WriteLine("No se encontraron coincidencias.");
    }

    static void CargarDesdeArchivo()
    {
        if (!File.Exists(archivoCSV)) return;

        string[] lineas = File.ReadAllLines(archivoCSV);
        contadorContactos = 0;

        for (int i = 0; i < lineas.Length && contadorContactos < MAX_CONTACTOS; i++)
        {
            string[] partes = lineas[i].Split(',');

            if (partes.Length == 4 && int.TryParse(partes[0], out int id))
            {
                agenda[contadorContactos] = new Contacto
                {
                    Id = id,
                    Nombre = partes[1],
                    Telefono = partes[2],
                    Email = partes[3]
                };
                contadorContactos++;
            }
        }
    }

    static void GuardarEnArchivo()
    {
        using (StreamWriter sw = new StreamWriter(archivoCSV))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
        Console.WriteLine("Datos guardados en archivo. Saliendo...");
    }
}
