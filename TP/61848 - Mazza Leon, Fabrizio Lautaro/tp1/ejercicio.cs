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

//IMPORTANTE SI NO SE COMPILA POR 2DA VEZ FINALIZAR EL PROCESO ejercicio1.exe Y VOLVER A COMPILAR
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class agenda_de_contactos
{
 
    static Contacto[] agenda = new Contacto[100];
    static int total_contactos = 0;
    static string archivo = "agenda.csv";

    static void AgregarContacto()
    {
        if (total_contactos >= 100)
        {
            Console.WriteLine("AGENDA COMPLETA.");
            return;
        }

        Contacto nuevo;

        nuevo.Id = total_contactos + 1;

        Console.Write("Nombre: "); nuevo.Nombre = Console.ReadLine();
        Console.Write("Teléfono: "); nuevo.Telefono = Console.ReadLine();
        Console.Write("Email: "); nuevo.Email = Console.ReadLine();

        agenda[total_contactos] = nuevo;

        total_contactos++;

        GuardarContactos();

        Console.WriteLine();
        Console.WriteLine("CONTACTO AGREGADO EXITOSAMENTE.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < total_contactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write("Nuevo Nombre: ");

                string nombre = Console.ReadLine();

                if (nombre != "") agenda[i].Nombre = nombre;

                Console.Write("Nuevo Teléfono: ");
                string telefono = Console.ReadLine();

                if (telefono != "") agenda[i].Telefono = telefono;

                Console.Write("Nuevo Email: ");
                string email = Console.ReadLine();

                if (email != "") agenda[i].Email = email;

                Console.WriteLine();
                Console.WriteLine("CONTACTO ACTUALIZADO.");
                return;
            }
        }
        Console.WriteLine("");
        Console.WriteLine("ID NO ENCONTRADO.");

    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a eliminar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < total_contactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < total_contactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                    agenda[j].Id = j + 1;
                }

                total_contactos--;
                
                Console.WriteLine("");
                Console.WriteLine("CONTACTO ELIMINADO.");
                return;
            }
        
        Console.WriteLine("");
        Console.WriteLine("ID NO ENCONTRADO.");

    }

    static void VerContactos()
    {

        Console.WriteLine("\nID  Nombre           Teléfono        Email");
        Console.WriteLine("------------------------------------------------");

        for (int i = 0; i < total_contactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-3} {agenda[i].Nombre,-15} {agenda[i].Telefono,-15} {agenda[i].Email,-20}");
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda (Puede ser Nombre, Telefono o Email): ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("\nID  Nombre           Teléfono        Email");
        Console.WriteLine("------------------------------------------------");

        for (int i = 0; i < total_contactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) || agenda[i].Telefono.ToLower().Contains(termino) || agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-3} {agenda[i].Nombre,-15} {agenda[i].Telefono,-15} {agenda[i].Email,-20}");
            }
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivo)) return;

        string[] lineas = File.ReadAllLines(archivo);

        for (int i = 0; i < lineas.Length && i < 100; i++)
        {
            string[] datos = lineas[i].Split(',');

            agenda[i].Id = int.Parse(datos[0]);
            agenda[i].Nombre = datos[1];
            agenda[i].Telefono = datos[2];
            agenda[i].Email = datos[3];

            total_contactos++;
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivo))
        {
            for (int i = 0; i < total_contactos; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
    }
}

CargarContactos();

while (true)
{

     Console.WriteLine("\n==== AGENDA DE CONTACTOS ====");
    Console.WriteLine();

    Console.WriteLine("1. Agregar contacto");
    Console.WriteLine("2. Modificar contacto");
    Console.WriteLine("3. Borrar contacto");
    Console.WriteLine("4. Listar contactos");
    Console.WriteLine("5. Buscar contacto");
    Console.WriteLine("6. Salir");
    Console.WriteLine();

    Console.Write("Seleccione una opción: ");
    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            AgregarContacto();
            break;

        case "2":
            ModificarContacto();
            break;

        case "3":
            BorrarContacto();
            break;

        case "4":
            VerContactos();
            break;

        case "5":
            BuscarContacto();
            break;

        case "6":
            GuardarContactos();
            return;

        default:
            Console.WriteLine("OPCIÓN NO VÁLIDA.");
            break;
    }
}
