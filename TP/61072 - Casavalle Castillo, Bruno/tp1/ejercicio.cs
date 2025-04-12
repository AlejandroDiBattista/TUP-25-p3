using static System.Console;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class MenuAgenda
{
    static int IdInicial = 1;
    static int totalContactos = 0;
    static int IdMaximos = 50;
    static Contacto[] contactos = new Contacto[IdMaximos];
    // static string rutaCSV = @".\agenda.csv";
    static void Main(string[] args)
    {

        // CargarContactosCSV();

        bool salir = false;
        while (!salir)
        {
            Clear();
            WriteLine("*********** Menu Agenda ***********");
            WriteLine("====== 1. Agregar contacto =======");
            WriteLine("====== 2. Modificar contacto ======");
            WriteLine("====== 3. Borrar contacto ========");
            WriteLine("====== 4. Listar contactos =======");
            WriteLine("======= 5. Buscar contacto =======");
            WriteLine("============ 6. Salir ===========");

            string opcion = ReadLine();

            switch (opcion)
            {
                case "1":
                    AgregarContacto();
                    break;
                case "2":
                    Modificar();
                    break;
                case "3":
                    Borrar();
                    break;
                case "4":
                    Cargar();
                    break;
                case "5":
                    Buscar();
                    break;
                case "6":
                    salir = true;
                    break;
                default:
                    WriteLine("Opción incorrecta.");
                    break;
            }

        }
    }

    static void AgregarContacto()
    {
        if (totalContactos >= contactos.Length)
        {
            Array.Resize(ref contactos, contactos.Length + 5);
        }

        Contacto nuevo;
        nuevo.Id = IdInicial++;
        Write("Nombre: "); nuevo.Nombre = ReadLine();
        Write("Teléfono: "); nuevo.Telefono = ReadLine();
        Write("Email: "); nuevo.Email = ReadLine();

        contactos[totalContactos++] = nuevo;

        // GuardarContactosCSV();

        WriteLine("Contacto agregado.");

        ReadKey();
    }

    static void Cargar()
    {
        if (totalContactos == 0)
        {
            WriteLine("No hay contactos guardados.");
        }
        else
        {
            WriteLine("======== Historial de Contactos ========");
            WriteLine("ID   Nombre          Teléfono       Email");
            WriteLine("-----------------------------------------");

            for (int i = 0; i < totalContactos; i++)
            {
                WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-15} {contactos[i].Telefono,-14} {contactos[i].Email}");
            }
        }
        ReadKey();
    }

    static void Buscar()
    {
        Write("Ingrese término de búsqueda: ");
        string termino = ReadLine().ToLower();
        bool encontrado = false;

        WriteLine("======== Resultados de Búsqueda ========");
        WriteLine("ID   Nombre          Teléfono       Email");
        WriteLine("-----------------------------------------");

        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-15} {contactos[i].Telefono,-14} {contactos[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            WriteLine("No se encontraron contactos.");
        }
        ReadKey();
    }

    static void Modificar()
    {
        Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(ReadLine(), out int id))
        {
            WriteLine("ID inválido.");
            ReadKey();
            return;
        }

        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                WriteLine("Ingrese los nuevos valores (deje en blanco para no cambiar):");

                Write("Nuevo nombre: ");
                string nombre = ReadLine();
                if (!string.IsNullOrEmpty(nombre)) contactos[i].Nombre = nombre;

                Write("Nuevo teléfono: ");
                string telefono = ReadLine();
                if (!string.IsNullOrEmpty(telefono)) contactos[i].Telefono = telefono;

                Write("Nuevo email: ");
                string email = ReadLine();
                if (!string.IsNullOrEmpty(email)) contactos[i].Email = email;

                // GuardarContactosCSV();
                WriteLine("Contacto modificado.");
                ReadKey();
                return;
            }
        }
        WriteLine("No se encontró un contacto con ese ID.");
        ReadKey();
    }

    static void Borrar()
    {
        Write("Ingrese el ID del contacto a eliminar: ");
        if (!int.TryParse(ReadLine(), out int id))
        {
            WriteLine("ID inválido.");
            ReadKey();
            return;
        }

        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }

                totalContactos--;

                // GuardarContactosCSV();
                WriteLine("Contacto eliminado.");
                ReadKey();
                return;
            }
        }
        WriteLine("No se encontró un contacto con ese ID.");
        ReadKey();
    }
    /*
        static void GuardarContactosCSV()
        {
            using (StreamWriter writer = new StreamWriter(rutaCSV))
            {
                for (int i = 0; i < totalContactos; i++)
                {
                    writer.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
                }
            }
        }

        static void CargarContactosCSV()
        {
            if (File.Exists(rutaCSV))
            {
                string[] lineas = File.ReadAllLines(rutaCSV);
                totalContactos = lineas.Length;
                contactos = new Contacto[totalContactos];

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split(',');
                    if (datos.Length == 4)
                    {
                        contactos[i].Id = int.Parse(datos[0]);
                        contactos[i].Nombre = datos[1];
                        contactos[i].Telefono = datos[2];
                        contactos[i].Email = datos[3];
                    }
                }
            }
        }
        */


    //Disculpe profe no pude hacer que se pueda conectar al archivo csv.
}