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
using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Teléfono;
    public string Email;
}

class Program
{
    static Contacto[] contactos = new Contacto[0];
    static string archivo = "agenda.csv";
    static int proximoId = 1;

    static void Main()
    {
        CargarContactos();
        MostrarMenu();
    }

    static void MostrarMenu()
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
        EjecutarOpcion(opcion);
    }

    static void EjecutarOpcion(string opcion)
    {
        switch (opcion)
        {
            case "1": AgregarContacto(); break;
            case "2": ModificarContacto(); break;
            case "3": BorrarContacto(); break;
            case "4": ListarContactos(); break;
            case "5": BuscarContacto(); break;
            case "0": GuardarContactos(); return;
            default: Console.WriteLine("Opción inválida."); break;
        }
        MostrarMenu();
    }

    static void AgregarContacto()
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Clear();

        Contacto nuevo = new Contacto { Id = proximoId++, Nombre = nombre, Teléfono = telefono, Email = email };
        Contacto[] nuevoArray = new Contacto[contactos.Length + 1];
        for (int i = 0; i < contactos.Length; i++)
        {
            nuevoArray[i] = contactos[i];
        }
        nuevoArray[contactos.Length] = nuevo;
        contactos = nuevoArray;

        Console.WriteLine("Contacto agregado con éxito.");
        Console.WriteLine("ID      : {0}", nuevo.Id);
        Console.WriteLine("Nombre  : {0}", nuevo.Nombre);
        Console.WriteLine("Teléfono: {0}", nuevo.Teléfono);
        Console.WriteLine("Email   : {0}", nuevo.Email);
    
        Console.WriteLine("Presione una tecla para continuar...");

        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == id)
            {
                Console.Write("Nuevo Nombre (deje vacío para no cambiar): ");
                string nombre = Console.ReadLine();
                Console.Write("Nuevo Teléfono: ");
                string telefono = Console.ReadLine();
                Console.Write("Nuevo Email: ");
                string email = Console.ReadLine();

                if (!string.IsNullOrEmpty(nombre)) contactos[i].Nombre = nombre;
                if (!string.IsNullOrEmpty(telefono)) contactos[i].Teléfono = telefono;
                if (!string.IsNullOrEmpty(email)) contactos[i].Email = email;
                
                Console.WriteLine("Contacto modificado.");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());
        int index = -1;
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == id)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Console.WriteLine("ID no encontrado.");
            Console.ReadKey();
            return;
        }

        Contacto[] nuevoArray = new Contacto[contactos.Length - 1];
        for (int i = 0, j = 0; i < contactos.Length; i++)
        {
            if (i != index)
            {
                nuevoArray[j++] = contactos[i];
            }
        }
        contactos = nuevoArray;

        Console.WriteLine("Contacto eliminado.");
        Console.ReadKey();
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n=== Lista de Contactos ===");
        Console.WriteLine("ID   NOMBRE                TELÉFONO       EMAIL");
        for (int i = 0; i < contactos.Length; i++)
        {
            Console.WriteLine("{0,-4} {1,-20} {2,-15} {3}", contactos[i].Id, contactos[i].Nombre, contactos[i].Teléfono, contactos[i].Email);
        }
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string term = Console.ReadLine().ToLower();
        Console.WriteLine("\nResultados de la búsqueda:");
        Console.WriteLine("ID   NOMBRE                TELÉFONO       EMAIL");
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(term) || contactos[i].Teléfono.Contains(term) || contactos[i].Email.ToLower().Contains(term))
            {
                Console.WriteLine("{0,-4} {1,-20} {2,-15} {3}", contactos[i].Id, contactos[i].Nombre, contactos[i].Teléfono, contactos[i].Email);
            }
        }
        Console.ReadKey();
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivo)) return;
        string[] lineas = File.ReadAllLines(archivo);
        contactos = new Contacto[lineas.Length];
        for (int i = 0; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split(',');
            if (datos.Length == 4)
            {
                contactos[i] = new Contacto { Id = int.Parse(datos[0]), Nombre = datos[1], Teléfono = datos[2], Email = datos[3] };
                proximoId = Math.Max(proximoId, int.Parse(datos[0]) + 1);
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivo))
        {
            for (int i = 0; i < contactos.Length; i++)
            {
                sw.WriteLine("{0},{1},{2},{3}", contactos[i].Id, contactos[i].Nombre, contactos[i].Teléfono, contactos[i].Email);
            }
        }
    }
}