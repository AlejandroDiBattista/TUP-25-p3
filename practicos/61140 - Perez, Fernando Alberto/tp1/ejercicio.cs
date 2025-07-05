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
Console.WriteLine("Hola, soy el ejercicio 1 del TP1 de la materia Programación 3");
Console.Write("Presionar una tecla para continuar...");
Console.ReadKey();struct Contacto
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
    static int nextId = 1;
    const string archivo = "agenda.csv";

    static void Main()
    {
        CargarDesdeArchivo();
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
                case "0":
                    GuardarEnArchivo();
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void AgregarContacto()
    {
        if (totalContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("Agenda llena. No se pueden agregar más contactos.");
            Console.ReadKey();
            return;
        }

        Contacto nuevo;
        nuevo.Id = nextId++;
        Console.Write("Nombre: "); nuevo.Nombre = Console.ReadLine();
        Console.Write("Teléfono: "); nuevo.Telefono = Console.ReadLine();
        Console.Write("Email: "); nuevo.Email = Console.ReadLine();
        
        agenda[totalContactos++] = nuevo;
        Console.WriteLine($"Contacto agregado con ID = {nuevo.Id}");
        Console.ReadKey();
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.WriteLine($"Datos actuales => Nombre: {agenda[i].Nombre}, Teléfono: {agenda[i].Telefono}, Email: {agenda[i].Email}");
                Console.Write("Nuevo nombre (Enter para mantener): ");
                string nombre = Console.ReadLine();
                Console.Write("Nuevo teléfono (Enter para mantener): ");
                string telefono = Console.ReadLine();
                Console.Write("Nuevo email (Enter para mantener): ");
                string email = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nombre)) agenda[i].Nombre = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) agenda[i].Telefono = telefono;
                if (!string.IsNullOrWhiteSpace(email)) agenda[i].Email = email;

                Console.WriteLine("Contacto modificado con éxito.");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                totalContactos--;
                Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
        Console.ReadKey();
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n=== Lista de Contactos ===");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("----------------------------------------------------");
        for (int i = 0; i < totalContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
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
        Console.WriteLine("----------------------------------------------------");
        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) || agenda[i].Telefono.Contains(termino) || agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
            }
        }
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void CargarDesdeArchivo()
    {
        if (!File.Exists(archivo)) return;
        string[] lineas = File.ReadAllLines(archivo);
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 4 && int.TryParse(datos[0], out int id))
            {
                agenda[totalContactos++] = new Contacto { Id = id, Nombre = datos[1], Telefono = datos[2], Email = datos[3] };
                nextId = Math.Max(nextId, id + 1);
            }
        }
    }

    static void GuardarEnArchivo()
    {
        using (StreamWriter sw = new StreamWriter(archivo))
        {
            for (int i = 0; i < totalContactos; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
    }
}