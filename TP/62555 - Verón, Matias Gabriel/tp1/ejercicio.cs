using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)


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
    static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
    static int cantidadContactos = 0;
    static int proximoId = 1;
    static string archivoSCV = "agenda.csv";

    static void Main(string[] args)
    {
        CargarContactos();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== AGENDA DE CONTACTOS =====");
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contacto");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("0) Salir");

            Console.WriteLine("Seleccione una opcion: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "0": GuardarContacto(); return;
                default: Console.WriteLine("Opcion no valida."); break;
            }
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivoSCV)) return;
        string[] lineas = File.ReadAllLines(archivoSCV);
        foreach (string linea in lineas) 
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 4)
            {
                contactos[cantidadContactos].Id = int.Parse(datos[0]);
                contactos[cantidadContactos].Nombre = datos[1];
                contactos[cantidadContactos].Telefono = datos[2];
                contactos[cantidadContactos].Email = datos[3];
                cantidadContactos++;
                proximoId++;
            }
        }
    }

    static void GuardarContacto()
    {
        using (StreamWriter sw = new StreamWriter(archivoSCV))
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
    }


    static void AgregarContacto()
    {
        if (cantidadContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("No se pueden agregar mas contactos.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Telefono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        contactos[cantidadContactos] = new Contacto { Id = proximoId++, Nombre = nombre, Telefono = telefono, Email = email };
        cantidadContactos++;
        Console.WriteLine("Contacto agregado con exito.");
        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                Console.Write($"Nuevo nombre ({contactos[i].Nombre}): ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre)) contactos[i].Nombre = nuevoNombre;

                Console.Write($"Nuevo telefono ({contactos[i].Telefono}): ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre)) contactos[i].Telefono = nuevoTelefono;

                Console.Write($"Nuevo email ({contactos[i].Email}): ");
                string nuevoEmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre)) contactos[i].Email = nuevoEmail;

                Console.WriteLine("Contacto modificado con exito.");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < cantidadContactos - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }

                cantidadContactos--;
                Console.WriteLine("Contacto eliminado con exito.");
                Console.ReadKey(); 
                return;    
            }
        }
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n=== Lista de contactos ===");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("--------------------------------------------------");

        for (int i = 0; i < cantidadContactos; i++)
        {
            Console.WriteLine($"{contactos[i].Id, -5} {contactos[i].Nombre, -20} {contactos[i].Telefono,-15} {contactos[i].Email,-25}");
        }
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese un termino de busqueda: ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("\nResultados de la busqueda: ");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("--------------------------------------------------");

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-25}");
            }
        }
        Console.ReadKey();
    }
}

