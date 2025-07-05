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
    static string archivo = "agenda.csv";

    static void Main()
    {
        CargarContactos();
        int opcion;
        do
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
            opcion = int.Parse(Console.ReadLine());

                      if (opcion == 1)
            {
                AgregarContacto();
            }
            else if (opcion == 2)
            {
                ModificarContacto();
            }
            else if (opcion == 3)
            {
                BorrarContacto();
            }
            else if (opcion == 4)
            {
                ListarContactos();
            }
            else if (opcion == 5)
            {
                BuscarContacto();
            }
            else if (opcion == 0)
            {
                GuardarContactos();
                Console.WriteLine("Saliendo...");
            }
            else
            {
                Console.WriteLine("Opción inválida");
            }
            if (opcion != 0) Console.ReadKey();
        } while (opcion != 0);
    }

    static void CargarContactos()
    {
        if (File.Exists(archivo))
        {
            string[] lineas = File.ReadAllLines(archivo);
            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(',');
                if (contador < MAX_CONTACTOS)
                {
                    agenda[contador].Id = int.Parse(datos[0]);
                    agenda[contador].Nombre = datos[1];
                    agenda[contador].Telefono = datos[2];
                    agenda[contador].Email = datos[3];
                    contador++;
                }
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

        agenda[contador].Id = (contador == 0) ? 1 : agenda[contador - 1].Id + 1;
        agenda[contador].Nombre = nombre;
        agenda[contador].Telefono = telefono;
        agenda[contador].Email = email;
        contador++;
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
                Console.WriteLine($"Actual: {agenda[i].Nombre}, {agenda[i].Telefono}, {agenda[i].Email}");
                Console.Write("Nuevo Nombre (dejar vacío para mantener): ");
                string nombre = Console.ReadLine();
                Console.Write("Nuevo Teléfono: ");
                string telefono = Console.ReadLine();
                Console.Write("Nuevo Email: ");
                string email = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nombre)) agenda[i].Nombre = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) agenda[i].Telefono = telefono;
                if (!string.IsNullOrWhiteSpace(email)) agenda[i].Email = email;
                Console.WriteLine("Contacto modificado.");
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese ID del contacto a borrar: ");
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
                Console.WriteLine("Contacto eliminado.");
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("ID    Nombre               Teléfono       Email");
        Console.WriteLine("--------------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email,-30}");
        }
    }

    static void BuscarContacto() 
    {
        Console.Write("Ingrese un término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        Console.WriteLine("ID    Nombre               Teléfono       Email");
        Console.WriteLine("--------------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) || agenda[i].Telefono.Contains(termino) || agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email,-30}");
            }
        
        }
    }
}
