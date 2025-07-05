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
    static int cantidadContactos = 0;
    static int proximoId = 1;
    static string archivo = "agenda.csv";

    static void Main()
    {
        LeerArchivo();

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

            if (opcion == "1") AgregarContacto();
            else if (opcion == "2") ModificarContacto();
            else if (opcion == "3") BorrarContacto();
            else if (opcion == "4") ListarContactos();
            else if (opcion == "5") BuscarContacto();
            else if (opcion == "0") break;
        }

        GuardarArchivo();
        Console.WriteLine("Saliendo de la aplicación...");
    }

    static void LeerArchivo()
    {
        if (File.Exists(archivo))
        {
            string[] lineas = File.ReadAllLines(archivo);
            for (int i = 0; i < lineas.Length && cantidadContactos < MAX_CONTACTOS; i++)
            {
                string[] partes = lineas[i].Split(',');
                if (partes.Length == 4)
                {
                    contactos[cantidadContactos].Id = int.Parse(partes[0]);
                    contactos[cantidadContactos].Nombre = partes[1];
                    contactos[cantidadContactos].Telefono = partes[2];
                    contactos[cantidadContactos].Email = partes[3];
                    cantidadContactos++;

                    if (contactos[cantidadContactos - 1].Id >= proximoId)
                        proximoId = contactos[cantidadContactos - 1].Id + 1;
                }
            }
        }
    }

    static void GuardarArchivo()
    {
        string[] lineas = new string[cantidadContactos];
        for (int i = 0; i < cantidadContactos; i++)
        {
            lineas[i] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
        }
        File.WriteAllLines(archivo, lineas);
    }

    static void AgregarContacto()
    {
        Console.Clear();
        Console.WriteLine("=== Agregar Contacto ===");
        if (cantidadContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("No se pueden agregar más contactos.");
            Console.ReadKey();
            return;
        }

        Console.Write("Nombre   : ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono : ");
        string telefono = Console.ReadLine();
        Console.Write("Email    : ");
        string email = Console.ReadLine();

        contactos[cantidadContactos].Id = proximoId++;
        contactos[cantidadContactos].Nombre = nombre;
        contactos[cantidadContactos].Telefono = telefono;
        contactos[cantidadContactos].Email = email;
        cantidadContactos++;

        Console.WriteLine($"Contacto agregado con ID = {proximoId - 1}");
        Console.Write("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Clear();
        Console.WriteLine("=== Modificar Contacto ===");
        Console.Write("Ingrese el ID del contacto a modificar: ");
        string entrada = Console.ReadLine();
        int id;
        if (!int.TryParse(entrada, out id))
        {
            Console.WriteLine("ID inválido.");
            Console.ReadKey();
            return;
        }

        int index = BuscarIndicePorId(id);
        if (index == -1)
        {
            Console.WriteLine("No se encontró el contacto.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Datos actuales => Nombre: {contactos[index].Nombre}, Teléfono : {contactos[index].Telefono}, Email: {contactos[index].Email}");
        Console.WriteLine("(Deje el campo en blanco para no modificar)");

        Console.Write("Nombre    : ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono  : ");
        string telefono = Console.ReadLine();
        Console.Write("Email     : ");
        string email = Console.ReadLine();

        if (nombre != "") contactos[index].Nombre = nombre;
        if (telefono != "") contactos[index].Telefono = telefono;
        if (email != "") contactos[index].Email = email;

        Console.WriteLine("Contacto modificado con éxito.");
        Console.Write("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Clear();
        Console.WriteLine("=== Borrar Contacto ===");
        Console.Write("Ingrese el ID del contacto a borrar: ");
        string entrada = Console.ReadLine();
        int id;
        if (!int.TryParse(entrada, out id))
        {
            Console.WriteLine("ID inválido.");
            Console.ReadKey();
            return;
        }

        int index = BuscarIndicePorId(id);
        if (index == -1)
        {
            Console.WriteLine("No se encontró el contacto.");
            Console.ReadKey();
            return;
        }

        for (int i = index; i < cantidadContactos - 1; i++)
        {
            contactos[i] = contactos[i + 1];
        }
        cantidadContactos--;

        Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
        Console.Write("Presione una tecla para continuar...");
        Console.ReadKey();
    }

    static void ListarContactos()
    {
        Console.Clear();
        Console.WriteLine("=== Lista de Contactos ===");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
        for (int i = 0; i < cantidadContactos; i++)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}",
                contactos[i].Id,
                contactos[i].Nombre,
                contactos[i].Telefono,
                contactos[i].Email);
        }

        Console.Write("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Clear();
        Console.WriteLine("=== Buscar Contacto ===");
        Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("\nResultados de la búsqueda:");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");

        for (int i = 0; i < cantidadContactos; i++)
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

        Console.Write("\nPresione una tecla para continuar...");
        Console.ReadKey();
    }

    static int BuscarIndicePorId(int id)
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
                return i;
        }
        return -1;
    }
}
