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
Console.ReadKey();

// el programa fue hecho en Visual Studio Commuity y transladado aca 
// es una copia del anterior que habia subido mal asi que ahora vuelvo a subirlo pero esta vez bien



class Program
{
    const int maximo = 100;
    const string AGENDA = "agenda.csv";

    struct Contacto
    {
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Email;
    }

    static void Main(string[] args)
    {
        Contacto[] agenda = new Contacto[maximo];
        int numContactos = 0;
        int ultimoId = 0;

        // Cargar contactos desde archivo
        CargarContactos(ref agenda, ref numContactos, ref ultimoId);

        bool continuar = true;
        while (continuar)
        {
            Console.Clear();
            MostrarMenu();
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    AgregarContacto(ref agenda, ref numContactos, ref ultimoId);
                    break;
                case "2":
                    ModificarContacto(agenda, numContactos);
                    break;
                case "3":
                    BorrarContacto(ref agenda, ref numContactos);
                    break;
                case "4":
                    ListarContactos(agenda, numContactos);
                    break;
                case "5":
                    BuscarContacto(agenda, numContactos);
                    break;
                case "0":
                    GuardarContactos(agenda, numContactos);
                    continuar = false;
                    Console.WriteLine("\nSaliendo de la aplicación");
                    break;
                default:
                    Console.WriteLine("\nOpción no válida.");
                    Console.WriteLine("Presione cualquier tecla para continuar");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void MostrarMenu()
    {
        Console.WriteLine(" AGENDA DE CONTACTOS ");
        Console.WriteLine("1) Agregar contacto");
        Console.WriteLine("2) Modificar contacto");
        Console.WriteLine("3) Borrar contacto");
        Console.WriteLine("4) Listar contactos");
        Console.WriteLine("5) Buscar contacto");
        Console.WriteLine("0) Salir");
        Console.Write("Seleccione una opción: ");
    }

    static void AgregarContacto(ref Contacto[] agenda, ref int numContactos, ref int ultimoId)
    {
        Console.Clear();
        Console.WriteLine(" Agregar Contacto ");

        if (numContactos >= maximo)
        {
            Console.WriteLine("\n agenda  llena.");
            Console.WriteLine("Presione cualquier tecla para continuar");
            Console.ReadKey();
            return;
        }

        ultimoId++;
        Console.Write("Nombre   : ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono : ");
        string telefono = Console.ReadLine();
        Console.Write("Email    : ");
        string email = Console.ReadLine();

        agenda[numContactos].Id = ultimoId;
        agenda[numContactos].Nombre = nombre;
        agenda[numContactos].Telefono = telefono;
        agenda[numContactos].Email = email;
        numContactos++;

        Console.WriteLine($"Contacto agregado con ID = {ultimoId}");
        Console.WriteLine("Presione cualquier tecla para continuar");
        Console.ReadKey();
    }

    static void ModificarContacto(Contacto[] agenda, int numContactos)
    {
        Console.Clear();
        Console.WriteLine(" Modificar Contacto ");
        Console.Write("Ingrese el ID del contacto a modificar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nID inexistente.");
            Console.WriteLine("Presione cualquier tecla para continuar");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < numContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.WriteLine($"Datos actuales: Nombre: {agenda[i].Nombre}, Teléfono: {agenda[i].Telefono}, Email: {agenda[i].Email}");

                Console.Write("Nombre    : ");
                string nombre = Console.ReadLine();
                if (nombre != "") agenda[i].Nombre = nombre;

                Console.Write("Teléfono  : ");
                string telefono = Console.ReadLine();
                if (telefono != "") agenda[i].Telefono = telefono;

                Console.Write("Email     : ");
                string email = Console.ReadLine();
                if (email != "") agenda[i].Email = email;

                Console.WriteLine("\nContacto modificado ");
                Console.WriteLine("Presione cualquier tecla para continuar");
                Console.ReadKey();
                return;
            }
        }

        Console.WriteLine("\nID inexistente");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void BorrarContacto(ref Contacto[] agenda, ref int numContactos)
    {
        Console.Clear();
        Console.WriteLine(" Borrar Contacto ");
        Console.Write("Ingrese el ID del contacto que quiere borrar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nID inexistente.");
            Console.WriteLine("Presione cualquier tecla para continuar");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < numContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < numContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                numContactos--;
                Console.WriteLine($"Contacto con ID={id} eliminado");
                Console.WriteLine("Presione cualquier tecla para continuar");
                Console.ReadKey();
                return;
            }
        }

        Console.WriteLine("\nNo se encontró un contacto con ese ID.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void ListarContactos(Contacto[] agenda, int numContactos)
    {
        Console.Clear();
        Console.WriteLine(" Lista de Contactos");
        MostrarEncabezadoTabla();

        for (int i = 0; i < numContactos; i++)
        {
            MostrarContacto(agenda[i]);
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar");
        Console.ReadKey();
    }

    static void BuscarContacto(Contacto[] agenda, int numContactos)
    {
        Console.Clear();
        Console.WriteLine(" Buscar Contacto ");
        Console.Write("Busqueda X (nombre, teléfono o email): ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("\nResultados de la búsqueda:");
        MostrarEncabezadoTabla();

        for (int i = 0; i < numContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                MostrarContacto(agenda[i]);
            }
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar");
        Console.ReadKey();
    }

    static void MostrarEncabezadoTabla()
    {
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
    }

    static void MostrarContacto(Contacto c)
    {
        Console.WriteLine($"{c.Id,-5} {c.Nombre,-20} {c.Telefono,-13} {c.Email,-25}");
    }

    static void CargarContactos(ref Contacto[] agenda, ref int numContactos, ref int ultimoId)
    {
        if (!File.Exists(AGENDA)) return;

        string[] lineas = File.ReadAllLines(AGENDA);
        for (int i = 0; i < lineas.Length && numContactos < maximo; i++)
        {
            string[] campos = lineas[i].Split(',');
            if (campos.Length == 4)
            {
                agenda[numContactos].Id = int.Parse(campos[0]);
                agenda[numContactos].Nombre = campos[1];
                agenda[numContactos].Telefono = campos[2];
                agenda[numContactos].Email = campos[3];

                if (agenda[numContactos].Id > ultimoId)
                {
                    ultimoId = agenda[numContactos].Id;
                }

                numContactos++;
            }
        }
    }

    static void GuardarContactos(Contacto[] agenda, int numContactos)
    {
        string[] lineas = new string[numContactos];
        for (int i = 0; i < numContactos; i++)
        {
            lineas[i] = $"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}";
        }
        File.WriteAllLines(AGENDA, lineas);
    }
}
