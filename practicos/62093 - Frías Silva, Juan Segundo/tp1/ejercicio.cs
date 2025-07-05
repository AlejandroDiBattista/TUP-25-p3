
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
    static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
    static int contador = 0;
    static string archivo = "agenda.csv";

    static void Main()
    {
        CargarContactos();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== AGENDA DE CONTACTOS =====");
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contactos");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("0) Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "0": GuardarContactos(); return;
                default: Console.WriteLine("Opción inválida. Presione una tecla para continuar..."); Console.ReadKey(); break;
            }
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivo)) return;
        string[] lineas = File.ReadAllLines(archivo);
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 4 && contador < MAX_CONTACTOS)
            {
                contactos[contador++] = new Contacto
                {
                    Id = int.Parse(datos[0]),
                    Nombre = datos[1],
                    Telefono = datos[2],
                    Email = datos[3]
                };
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivo))
        {
            for (int i = 0; i < contador; i++)
            {
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
    }

    static void AgregarContacto()
    {
        if (contador >= MAX_CONTACTOS)
        {
            Console.WriteLine("La agenda está llena.");
            Console.ReadKey();
            return;
        }
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        contactos[contador++] = new Contacto { Id = contador, Nombre = nombre, Telefono = telefono, Email = email };
        Console.WriteLine("Contacto agregado con éxito.");
        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;
        for (int i = 0; i < contador; i++)
        {
            if (contactos[i].Id == id)
            {
                Console.Write($"Nombre ({contactos[i].Nombre}): ");
                string nombre = Console.ReadLine();
                Console.Write($"Teléfono ({contactos[i].Telefono}): ");
                string telefono = Console.ReadLine();
                Console.Write($"Email ({contactos[i].Email}): ");
                string email = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nombre)) contactos[i].Nombre = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) contactos[i].Telefono = telefono;
                if (!string.IsNullOrWhiteSpace(email)) contactos[i].Email = email;
                Console.WriteLine("Contacto modificado con éxito.");
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
        if (!int.TryParse(Console.ReadLine(), out int id)) return;
        for (int i = 0; i < contador; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < contador - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }
                contador--;
                Console.WriteLine("Contacto eliminado con éxito.");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n=== Lista de Contactos ===");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email}");
        }
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese un término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        Console.WriteLine("\nResultados de la búsqueda:");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < contador; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) || contactos[i].Telefono.Contains(termino) || contactos[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email}");
            }
        }
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}
