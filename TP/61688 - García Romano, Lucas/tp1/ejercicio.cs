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
    static int contador = 0;
    const string archivo = "agenda.csv";

    static void Main()
    {
        CargarContactos();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Menú de la Agenda");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "6": GuardarContactos(); return;
                default: Console.WriteLine("Opción no válida"); break;
            }
            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
        }
    }

    static void AgregarContacto()
    {
        if (contador >= MAX_CONTACTOS)
        {
            Console.WriteLine("La agenda está llena.");
            return;
        }
        Contacto nuevo;
        nuevo.Id = contador + 1;
        Console.Write("Ingrese nombre: ");
        nuevo.Nombre = Console.ReadLine();
        Console.Write("Ingrese teléfono: ");
        nuevo.Telefono = Console.ReadLine();
        Console.Write("Ingrese email: ");
        nuevo.Email = Console.ReadLine();
        
        agenda[contador++] = nuevo;
        Console.WriteLine("Contacto agregado con éxito.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write("Nuevo nombre (Enter para mantener): ");
                string nombre = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nombre)) agenda[i].Nombre = nombre;
                
                Console.Write("Nuevo teléfono (Enter para mantener): ");
                string telefono = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(telefono)) agenda[i].Telefono = telefono;
                
                Console.Write("Nuevo email (Enter para mantener): ");
                string email = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(email)) agenda[i].Email = email;
                
                Console.WriteLine("Contacto modificado correctamente.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese ID del contacto a eliminar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < contador - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                contador--;
                Console.WriteLine("Contacto eliminado correctamente.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("\nID  Nombre           Teléfono        Email");
        Console.WriteLine("-----------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-4}{agenda[i].Nombre,-15}{agenda[i].Telefono,-15}{agenda[i].Email}");
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        Console.WriteLine("\nID  Nombre           Teléfono        Email");
        Console.WriteLine("-----------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-4}{agenda[i].Nombre,-15}{agenda[i].Telefono,-15}{agenda[i].Email}");
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
            if (datos.Length == 4)
            {
                agenda[contador].Id = int.Parse(datos[0]);
                agenda[contador].Nombre = datos[1];
                agenda[contador].Telefono = datos[2];
                agenda[contador].Email = datos[3];
                contador++;
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivo))
        {
            for (int i = 0; i < contador; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
        Console.WriteLine("Contactos guardados. Saliendo...");
    }
}
