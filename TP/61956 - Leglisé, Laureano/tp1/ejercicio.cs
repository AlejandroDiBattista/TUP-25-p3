using System; // Para usar la consola (Console)
using System.IO; // Para leer y escribir archivos

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Apellido;
    public string Telefono;
    public string Email;

    public Contacto(int id, string nombre, string apellido, string telefono, string email)
    {
        Id = id;
        Nombre = nombre ?? "";
        Apellido = apellido ?? "";
        Telefono = telefono ?? "";
        Email = email ?? "";
    }
}

class Program
{
    const int MAX_CONTACTOS = 100;
    static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
    static int contadorContactos = 0;
    static string archivoCSV = "agenda.csv";

    static void Main()
    {
        CargarContactos();

        while (true)
        {
            Console.WriteLine("\n--- AGENDA DE CONTACTOS ---");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine()?.Trim() ?? "";
            Console.Clear();

            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "6": GuardarContactos(); return;
                default: Console.WriteLine("Opción no válida."); break;
            }
        }
    }

    static void AgregarContacto()
    {
        if (contadorContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("Agenda llena.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(nombre)) { Console.WriteLine("Nombre no puede estar vacío."); return; }

        Console.Write("Apellido: ");
        string apellido = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(apellido)) { Console.WriteLine("Apellido no puede estar vacío."); return; }

        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(telefono)) { Console.WriteLine("Teléfono no puede estar vacío."); return; }

        Console.Write("Email: ");
        string email = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(email)) { Console.WriteLine("Email no puede estar vacío."); return; }

        Contacto nuevo = new Contacto(
            (contadorContactos == 0) ? 1 : agenda[contadorContactos - 1].Id + 1,
            nombre, apellido, telefono, email
        );

        agenda[contadorContactos++] = nuevo;
        Console.WriteLine("Contacto agregado.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write($"Nuevo nombre ({agenda[i].Nombre}): ");
                string? nombre = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;

                Console.Write($"Nuevo apellido ({agenda[i].Apellido}): ");
                string? apellido = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(apellido)) agenda[i].Apellido = apellido;

                Console.Write($"Nuevo teléfono ({agenda[i].Telefono}): ");
                string? telefono = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;

                Console.Write($"Nuevo email ({agenda[i].Email}): ");
                string? email = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;

                Console.WriteLine("Contacto modificado.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < contadorContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                contadorContactos--;
                agenda[contadorContactos] = default;
                Console.WriteLine("Contacto eliminado.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        if (contadorContactos == 0)
        {
            Console.WriteLine("No hay contactos.");
            return;
        }

        Console.WriteLine("ID  | Nombre       | Apellido       | Teléfono       | Email");
        Console.WriteLine("------------------------------------------------------------");

        for (int i = 0; i < contadorContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-3} | {agenda[i].Nombre,-12} | {agenda[i].Apellido,-12} | {agenda[i].Telefono,-14} | {agenda[i].Email}");
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine()?.ToLower().Trim() ?? "";
        bool encontrado = false;

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Apellido.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id} | {agenda[i].Nombre} {agenda[i].Apellido} | {agenda[i].Telefono} | {agenda[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado) Console.WriteLine("No se encontraron coincidencias.");
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivoCSV))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Apellido},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
        Console.WriteLine("Datos guardados.");
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivoCSV)) return;

        using (StreamReader sr = new StreamReader(archivoCSV))
        {
            string linea;
            while ((linea = sr.ReadLine()!) != null)
            {
                string[] datos = linea.Split(',');
                if (datos.Length == 5 && contadorContactos < MAX_CONTACTOS)
                {
                    if (int.TryParse(datos[0], out int id))
                    {
                        agenda[contadorContactos] = new Contacto(id, datos[1], datos[2], datos[3], datos[4]);
                        contadorContactos++;
                    }
                }
            }
        }
    }
}
