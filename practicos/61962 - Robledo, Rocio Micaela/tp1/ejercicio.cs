using System;
using System.IO

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Agenda
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
                default: Console.WriteLine("Opción inválida"); break;
            }
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivo)) return;
        string[] lineas = File.ReadAllLines(archivo);
        foreach (string linea in lineas)
        {
            string[] partes = linea.Split(',');
            if (partes.Length == 4)
            {
                contactos[contador++] = new Contacto
                {
                    Id = int.Parse(partes[0]),
                    Nombre = partes[1],
                    Telefono = partes[2],
                    Email = partes[3]
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
            Console.WriteLine("Agenda llena.");
            return;
        }
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        contactos[contador] = new Contacto { Id = contador + 1, Nombre = nombre, Telefono = telefono, Email = email };
        contador++;
        Console.WriteLine("Contacto agregado con éxito.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());
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
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());
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
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-30}");
        }
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese un término de búsqueda: ");
        string busqueda = Console.ReadLine().ToLower();
        for (int i = 0; i < contador; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(busqueda) ||
                contactos[i].Telefono.Contains(busqueda) ||
                contactos[i].Email.ToLower().Contains(busqueda))
            {
                Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-30}");
            }
        }
        Console.ReadKey();
    }
}
