using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program
{
    const int MaxContactos = 100;
    static Contacto[] agenda = new Contacto[MaxContactos];
    static int contadorContactos = 0;

    static void Main()
    {
        CargarAgenda();
        while (true)
        {
            MostrarMenu();
            string opcion = Console.ReadLine();
            Console.Clear();

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
                    ListarContactos();
                    break;
                case "5":
                    BuscarContacto();
                    break;
                case "0":
                    CargarAgenda();
                    Console.WriteLine("Saliendo de la aplicación...\n");
                    return;
                default:
                    Console.WriteLine("\nOpción no válida, presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }


    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("_______________________________");
        Console.WriteLine("      AGENDA DE CONTACTOS      ");
        Console.WriteLine("_______________________________");
        Console.WriteLine("1) Agregar contacto");
        Console.WriteLine("2) Modificar contacto");
        Console.WriteLine("3) Borrar contacto");
        Console.WriteLine("4) Listar contactos");
        Console.WriteLine("5) Buscar contacto");
        Console.WriteLine("0) Salir");
        Console.WriteLine("_______________________________");
        Console.Write("Seleccione una opción: ");
    }

///VALIDACIONES///
    static bool ValidarNombre(string nombre)
    {
        foreach (char c in nombre)
        {
            if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                return false;
        }
        return true;
    }

    static bool ValidarTelefono(string telefono)
    {
        return telefono.Length == 10 && long.TryParse(telefono, out _);
    }

    static bool ValidarEmail(string email)
    {
        return email.Contains("@")&& email.Contains("com");
    }

/// CARGAR Y GUARDAR CONTACTOS///
    static void CargarAgenda()
    {
        try
        {
            if (File.Exists("agenda.csv"))
            {
                string[] lineas = File.ReadAllLines("agenda.csv");
                for (int i = 0; i < lineas.Length && contadorContactos < MaxContactos; i++)
                {
                    string[] datos = lineas[i].Split(',');
                    if (datos.Length == 4)
                    {
                        agenda[contadorContactos] = new Contacto
                        {
                            Id = int.Parse(datos[0]),
                            Nombre = datos[1],
                            Telefono = datos[2],
                            Email = datos[3]
                        };
                        contadorContactos++;
                    }
                }
                Console.WriteLine("Contactos cargados exitosamente.");
            }
            else
            {
                Console.WriteLine("No se encontró el archivo 'agenda.csv'. Se creará uno nuevo al guardar.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar contactos: {ex.Message}");
        }
    }


    static void GuardarContactos()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("agenda.csv"))
            {
                for (int i = 0; i < contadorContactos; i++)
                {
                    writer.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
                }
            }
            Console.WriteLine("Contactos guardados exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar contactos: {ex.Message}");
        }
    }

/// BORRAR CONTACTOS///

    static void BorrarContacto()
    {
        Console.Clear();
        if (contadorContactos == 0)
        {
            Console.WriteLine("No hay contactos para eliminar. Presione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Lista de contactos:");
        for (int i = 0; i < contadorContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id}. {agenda[i].Nombre} - {agenda[i].Telefono} - {agenda[i].Email}");
        }

        int id;
        Console.Write("Ingrese el ID del contacto que desea eliminar: ");
        while (!int.TryParse(Console.ReadLine(), out id) || id < 1 || id > contadorContactos)
        {
            Console.WriteLine("ID no válido. Intente nuevamente.");
            Console.Write("Ingrese el ID del contacto que desea eliminar: ");
        }

        for (int i = id - 1; i < contadorContactos - 1; i++)
        {
            agenda[i] = agenda[i + 1];
        }
        contadorContactos--;
        GuardarContactos(); 

        Console.WriteLine("\nContacto eliminado con éxito. Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

/// MODIFICAR CONTACTOS ///
static void ModificarContacto()
{
    Console.Clear();
    if (contadorContactos == 0)
    {
        Console.WriteLine("No hay contactos para modificar. Presione cualquier tecla para continuar...");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Lista de contactos:");
    for (int i = 0; i < contadorContactos; i++)
    {
        Console.WriteLine($"{agenda[i].Id}. {agenda[i].Nombre} - {agenda[i].Telefono} - {agenda[i].Email}");
    }

    int id;
    Console.Write("Ingrese el ID del contacto que desea modificar: ");
    while (!int.TryParse(Console.ReadLine(), out id) || id < 1 || id > contadorContactos)
    {
        Console.WriteLine("ID no válido. Intente nuevamente.");
        Console.Write("Ingrese el ID del contacto que desea modificar: ");
    }

    // Modificación con validaciones
    bool nombreValido = false;
    while (!nombreValido)
    {
        Console.Write("Ingrese el nuevo nombre: ");
        string nuevoNombre = Console.ReadLine();
        if (ValidarNombre(nuevoNombre))
        {
            agenda[id - 1].Nombre = nuevoNombre;
            nombreValido = true;
        }
        else
        {
            Console.WriteLine("Nombre inválido. Solo se permiten letras y espacios.");
        }
    }

    bool telefonoValido = false;
    while (!telefonoValido)
    {
        Console.Write("Ingrese el nuevo teléfono (10 dígitos): ");
        string nuevoTelefono = Console.ReadLine();
        if (ValidarTelefono(nuevoTelefono))
        {
            agenda[id - 1].Telefono = nuevoTelefono;
            telefonoValido = true;
        }
        else
        {
            Console.WriteLine("Teléfono inválido. Solo se permiten 10 números.");
        }
    }

    bool emailValido = false;
    while (!emailValido)
    {
        Console.Write("Ingrese el nuevo email: ");
        string nuevoEmail = Console.ReadLine();
        if (ValidarEmail(nuevoEmail))
        {
            agenda[id - 1].Email = nuevoEmail;
            emailValido = true;
        }
        else
        {
            Console.WriteLine("Email inválido. Debe contener el símbolo '@' y/o contener '.com' para ser válido.");
        }
    }

    GuardarContactos();

    Console.WriteLine("\nContacto modificado con éxito. Presione cualquier tecla para continuar...");
    Console.ReadKey();
}


/// METODO PARA AGREGAR///
 static void AgregarContacto()
{
    Console.Clear();
    if (contadorContactos >= MaxContactos)
    {
        Console.WriteLine("La agenda está llena. Presione cualquier tecla para continuar...");
        Console.ReadKey();
        return;
    }

    // Inicializamos la variable correctamente
    Contacto nuevo = new Contacto();
    nuevo.Id = contadorContactos + 1;

    bool nombreValido = false;
    for (int i = 0; !nombreValido; i++)
    {
        Console.Write("Nombre o apodo: ");
        nuevo.Nombre = Console.ReadLine();
        if (ValidarNombre(nuevo.Nombre))
        {
            nombreValido = true;
        }
        else
        {
            Console.WriteLine("Nombre inválido. Solo se permiten letras y espacios.");
        }
    }

    bool telefonoValido = false;
    for (int i = 0; !telefonoValido; i++)
    {
        Console.Write("Teléfono (10 dígitos): ");
        nuevo.Telefono = Console.ReadLine();
        if (ValidarTelefono(nuevo.Telefono))
        {
            telefonoValido = true;
        }
        else
        {
            Console.WriteLine("Teléfono inválido. Solo se permiten 10 números.");
        }
    }

    bool emailValido = false;
    for (int i = 0; !emailValido; i++)
    {
        Console.Write("Email: ");
        nuevo.Email = Console.ReadLine();
        if (ValidarEmail(nuevo.Email))
        {
            emailValido = true;
        }
        else
        {
            Console.WriteLine("Email inválido. Debe contener el símbolo '@' y/o contener '.com' para ser válido.");
        }
    }

    agenda[contadorContactos] = nuevo;
    contadorContactos++;
    GuardarContactos();

    Console.WriteLine("\nContacto agregado con éxito. Presione cualquier tecla para continuar...");
    Console.ReadKey();
}


/// BUSCADOR DE CONTACTOS///
    static void BuscarContacto()
    {
        Console.Clear();
        Console.Write("Ingrese el nombre o parte del nombre del contacto a buscar: ");
        string terminoBusqueda = Console.ReadLine()?.ToLower();

        Console.Clear();
        Console.WriteLine("_______________________________");
        Console.WriteLine("     RESULTADOS DE BÚSQUEDA    ");
        Console.WriteLine("_______________________________");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("________________________________________________");

        bool encontrado = false;
        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(terminoBusqueda))
            {
                Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email,-25}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("\nNo se encontraron contactos que coincidan con el término de búsqueda.");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

/// LISTA DE CONTACTOS///

    static void ListarContactos()
    {
        Console.Clear();
        Console.WriteLine("_______________________________");
        Console.WriteLine("       LISTA DE CONTACTOS      ");
        Console.WriteLine("_______________________________");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("________________________________________________");
        for (int i = 0; i < contadorContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email,-25}");
        }
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}