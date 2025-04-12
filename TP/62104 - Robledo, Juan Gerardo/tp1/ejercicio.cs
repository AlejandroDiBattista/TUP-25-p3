using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public int Telefono;
    public string Email;

    public Contacto(int id, string nombre, int telefono, string email)
    {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }

    public void MostrarInformacion()
    {
        Console.WriteLine($"{Id,-5} {Nombre,-20} {Telefono,-12} {Email,-25}");
    }
}

class Program
{
    const int MaxContacto = 100;
    static Contacto[] agenda = new Contacto[MaxContacto];
    static int totalContacto = 0;
    static int IdInicio = 1;
    static string archivoCSV = "agenda.csv";

    static void Main(string[] args)
    {
        CargarDesdeArchivo();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("*********************************");
            Console.WriteLine("*      AGENDA DE CONTACTOS      *");
            Console.WriteLine("*********************************");
            Console.WriteLine("1. Agregar Contacto");
            Console.WriteLine("2. Modificar Contacto");
            Console.WriteLine("3. Borrar Contacto");
            Console.WriteLine("4. Listar Contacto");
            Console.WriteLine("5. Buscar Contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción del 1 al 6: ");

            if (int.TryParse(Console.ReadLine(), out int opcion))
            {
                switch (opcion)
                {
                    case 1:
                        AgregarContacto();
                        break;
                    case 2:
                        ModificarContacto();
                        break;
                    case 3:
                        BorrarContacto();
                        break;
                    case 4:
                        ListadoDeContactos();
                        break;
                    case 5:
                        BuscarContacto();
                        break;
                    case 6:
                        GuardarEnArchivo();
                        return;
                    default:
                        Console.WriteLine("Opción inválida, intente nuevamente.");
                        break;
                }
            }
        }
    }

    static void AgregarContacto()
    {
        if (totalContacto >= MaxContacto)
        {
            Console.WriteLine("No se pueden agregar más contactos.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        int telefono = int.Parse(Console.ReadLine());
        Console.Write("Email: ");
        string email = Console.ReadLine();

        agenda[totalContacto] = new Contacto(IdInicio++, nombre, telefono, email);
        totalContacto++;

        Console.WriteLine("Contacto agregado con éxito.");
        Console.ReadKey();
    }

    static void ListadoDeContactos()
    {
        Console.Clear();
        Console.WriteLine("=======================================================");
        Console.WriteLine("| ID   | Nombre               | Teléfono    | Email                    |");
        Console.WriteLine("=======================================================");
        for (int i = 0; i < totalContacto; i++)
        {
            Console.WriteLine($"| {agenda[i].Id,-5} | {agenda[i].Nombre,-20} | {agenda[i].Telefono,-10} | {agenda[i].Email,-25} |");
        }
        Console.WriteLine("=======================================================");
        Console.ReadKey();
    }

    static void GuardarEnArchivo()
    {
        using (StreamWriter sw = new StreamWriter(archivoCSV))
        {
            sw.WriteLine("ID   ,Nombre               ,Telefono    ,Email");
            for (int i = 0; i < totalContacto; i++)
            {
                sw.WriteLine($"{agenda[i].Id.ToString().PadRight(5)}," +
                             $"{agenda[i].Nombre.PadRight(20)}," +
                             $"{agenda[i].Telefono.ToString().PadRight(12)}," +
                             $"{agenda[i].Email.PadRight(25)}");
            }
        }

        Console.WriteLine("Agenda guardada en el archivo 'agenda.csv'.");
        Console.ReadKey();
    }

    static void CargarDesdeArchivo()
    {
        if (File.Exists(archivoCSV))
        {
            string[] lineas = File.ReadAllLines(archivoCSV);

            if (lineas.Length > 1)
            {
                for (int i = 1; i < lineas.Length; i++)
                {
                    var datos = lineas[i].Split(',');
                    agenda[totalContacto++] = new Contacto(
                        int.Parse(datos[0]),
                        datos[1],
                        int.Parse(datos[2]),
                        datos[3]
                    );
                }
            }
        }
    }

    static void ModificarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int idModificar = int.Parse(Console.ReadLine());

        bool encontrado = false;
        for (int i = 0; i < totalContacto; i++)
        {
            if (agenda[i].Id == idModificar)
            {
                encontrado = true;
                Console.WriteLine($"Contacto encontrado: {agenda[i].Nombre}, {agenda[i].Telefono}, {agenda[i].Email}");
                Console.Write("Nuevo nombre: ");
                agenda[i].Nombre = Console.ReadLine();
                Console.Write("Nuevo teléfono: ");
                agenda[i].Telefono = int.Parse(Console.ReadLine());
                Console.Write("Nuevo email: ");
                agenda[i].Email = Console.ReadLine();
                Console.WriteLine("Contacto modificado con éxito.");
                break;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("Contacto no encontrado.");
        }

        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int idBorrar = int.Parse(Console.ReadLine());

        bool encontrado = false;
        for (int i = 0; i < totalContacto; i++)
        {
            if (agenda[i].Id == idBorrar)
            {
                encontrado = true;
                for (int j = i; j < totalContacto - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                totalContacto--;
                Console.WriteLine("Contacto borrado con éxito.");
                break;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("Contacto no encontrado.");
        }

        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el nombre o teléfono del contacto a buscar: ");
        string buscar = Console.ReadLine().ToLower();

        bool encontrado = false;
        for (int i = 0; i < totalContacto; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(buscar) || agenda[i].Telefono.ToString().Contains(buscar))
            {
                encontrado = true;
                Console.WriteLine($"ID: {agenda[i].Id}, Nombre: {agenda[i].Nombre}, Teléfono: {agenda[i].Telefono}, Email: {agenda[i].Email}");
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontró ningún contacto.");
        }

        Console.ReadKey();
    }
}
