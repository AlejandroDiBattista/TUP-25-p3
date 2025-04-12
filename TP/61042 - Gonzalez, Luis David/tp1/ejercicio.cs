using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Agenda
{
    const int Contact_Max = 100;
    static Contacto[] agenda = new Contacto[Contact_Max];
    static int contador = 0;
    static string archivo = "agenda.csv";

    public static void Iniciar()
    {
        CargarDesdeArchivo();
    }

    public static void AgregarContacto(string nombre, string telefono, string email)
    {
        if (contador >= Contact_Max)
        {
            Console.WriteLine("Agenda llena. No se pueden agregar más contactos.");
            return;
        }

        Contacto nuevo;
        nuevo.Id = contador + 1;
        nuevo.Nombre = nombre;
        nuevo.Telefono = telefono;
        nuevo.Email = email;

        agenda[contador] = nuevo;
        contador++;
        Console.WriteLine("Contacto agregado con éxito.");
    }

    public static void ModificarContacto(int id, string nombre, string telefono, string email)
    {
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == id)
            {
                if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;
                if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;
                if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;
                Console.WriteLine("Contacto modificado correctamente.");
                return;
            }
        }
        Console.WriteLine("ID no encontrado en la agenda.");
    }

    public static void BorrarContacto(int id)
    {
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < contador - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                contador--;
                Console.WriteLine("Contacto eliminado con éxito.");
                return;
            }
        }
        Console.WriteLine("ID no encontrado en la agenda.");
    }

    public static void ListarContactos()
    {
        Console.WriteLine("\nLista de contactos:");
        Console.WriteLine("ID   Nombre           Teléfono         Email");
        Console.WriteLine("----------------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-4} {agenda[i].Nombre,-15} {agenda[i].Telefono,-15} {agenda[i].Email}");
        }
    }

    public static void BuscarContacto(string termino)
    {
        Console.WriteLine("\nResultados de búsqueda:");
        Console.WriteLine("ID   Nombre           Teléfono         Email");
        Console.WriteLine("----------------------------------------------------");
        bool encontrado = false;
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino.ToLower()) ||
                agenda[i].Telefono.ToLower().Contains(termino.ToLower()) ||
                agenda[i].Email.ToLower().Contains(termino.ToLower()))
            {
                Console.WriteLine($"{agenda[i].Id,-4} {agenda[i].Nombre,-15} {agenda[i].Telefono,-15} {agenda[i].Email}");
                encontrado = true;
            }
        }
        if (!encontrado)
        {
            Console.WriteLine("No se encontraron contactos con ese término de búsqueda.");
        }
    }

    public static void CargarDesdeArchivo()
    {
        if (!File.Exists(archivo)) return;
        string[] lineas = File.ReadAllLines(archivo);
        
        for (int i = 0; i < lineas.Length && i < Contact_Max; i++)
        {
            string[] datos = lineas[i].Split(',');
            if (datos.Length == 4)
            {
                agenda[i].Id = int.Parse(datos[0]);
                agenda[i].Nombre = datos[1];
                agenda[i].Telefono = datos[2];
                agenda[i].Email = datos[3];
                contador++;
            }
        }
    }

    public static void GuardarEnArchivo()
    {
        string[] lineas = new string[contador];
        for (int i = 0; i < contador; i++)
        {
            lineas[i] = $"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}";
        }
        File.WriteAllLines(archivo, lineas);
    }
}

Agenda.Iniciar();
while (true)
{
    Console.WriteLine("\nMenú de la Agenda:");
    Console.WriteLine("1. Agregar Contacto");
    Console.WriteLine("2. Modificar Contacto");
    Console.WriteLine("3. Borrar Contacto");
    Console.WriteLine("4. Listar Contactos");
    Console.WriteLine("5. Buscar Contacto");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opción: ");

    string opcion = Console.ReadLine();
    switch (opcion)
    {
        case "1":
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();
            Console.Write("Teléfono: ");
            string telefono = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Agenda.AgregarContacto(nombre, telefono, email);
            break;
        case "2":
            Console.Write("ID del contacto a modificar: ");
            int idMod = int.Parse(Console.ReadLine());
            Console.Write("Nuevo Nombre (Enter para mantener): ");
            string nuevoNombre = Console.ReadLine();
            Console.Write("Nuevo Teléfono (Enter para mantener): ");
            string nuevoTelefono = Console.ReadLine();
            Console.Write("Nuevo Email (Enter para mantener): ");
            string nuevoEmail = Console.ReadLine();
            Agenda.ModificarContacto(idMod, nuevoNombre, nuevoTelefono, nuevoEmail);
            break;
        case "3":
            Console.Write("ID del contacto a eliminar: ");
            int idBorrar = int.Parse(Console.ReadLine());
            Agenda.BorrarContacto(idBorrar);
            break;
        case "4":
            Agenda.ListarContactos();
            break;
        case "5":
            Console.Write("Ingrese término de búsqueda: ");
            string termino = Console.ReadLine();
            Agenda.BuscarContacto(termino);
            break;
        case "6":
            Agenda.GuardarEnArchivo();
            Console.WriteLine("Saliendo...");
            return;
        default:
            Console.WriteLine("Opción inválida. Intente de nuevo.");
            break;
    }
}