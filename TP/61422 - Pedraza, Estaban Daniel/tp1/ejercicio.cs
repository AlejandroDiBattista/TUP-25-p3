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

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Agenda
{
    const int MaxContactos = 100;
    public Contacto[] Contactos;
    public int Contador;

    public Agenda()
    {
        Contactos = new Contacto[MaxContactos];
        Contador = 0;
    }

    public void CargarContactos()
    {
        if (File.Exists("agenda.csv"))
        {
            try
            {
                string[] lineas = File.ReadAllLines("agenda.csv");
                for (int i = 0; i < lineas.Length && Contador < MaxContactos; i++)
                {
                    string[] datos = lineas[i].Split(',');
                    if (datos.Length == 4)
                    {
                        Contacto nuevo = new Contacto
                        {
                            Id = int.Parse(datos[0]),
                            Nombre = datos[1],
                            Telefono = datos[2],
                            Email = datos[3]
                        };
                        Contactos[Contador] = nuevo;
                        Contador++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mirá, hubo un error al cargar los contactos: {ex.Message}");
            }
        }
    }

    public void GuardarContactos()
    {
        try
        {
            string[] lineas = new string[Contador];
            for (int i = 0; i < Contador; i++)
            {
                lineas[i] = $"{Contactos[i].Id},{Contactos[i].Nombre},{Contactos[i].Telefono},{Contactos[i].Email}";
            }
            File.WriteAllLines("agenda.csv", lineas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hubo un error al guardar los contactos: {ex.Message}");
        }
    }

    public void AgregarContacto()
    {
        if (Contador >= MaxContactos)
        {
            Console.WriteLine("La agenda está llena, no podés agregar más contactos.");
            return;
        }

        Contacto nuevo = new Contacto
        {
            Id = Contador + 1
        };

        Console.Write("Nombre o apodo: ");
        nuevo.Nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        nuevo.Telefono = Console.ReadLine();
        Console.Write("Email: ");
        nuevo.Email = Console.ReadLine();

        Contactos[Contador] = nuevo;
        Contador++;
        Console.WriteLine("¡Contacto agregado con éxito :) !");
    }

    public void ListarContactos()
    {
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("------------------------------------------------");
        for (int i = 0; i < Contador; i++)
        {
            Console.WriteLine($"{Contactos[i].Id,-5} {Contactos[i].Nombre,-20} {Contactos[i].Telefono,-15} {Contactos[i].Email,-25}");
        }
    }

    public void BuscarContacto()
    {
        Console.Write("Decime el nombre o parte del nombre del contacto que querés buscar: ");
        string terminoBusqueda = Console.ReadLine()?.ToLower();
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("------------------------------------------------");

        bool encontrado = false;
        for (int i = 0; i < Contador; i++)
        {
            if (Contactos[i].Nombre.ToLower().Contains(terminoBusqueda))
            {
                Console.WriteLine($"{Contactos[i].Id,-5} {Contactos[i].Nombre,-20} {Contactos[i].Telefono,-15} {Contactos[i].Email,-25}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No encontré ningún contacto, fijate si escribiste bien.");
        }
    }

    public void ModificarContacto()
    {
        Console.Write("Decime el ID del contacto que querés modificar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            for (int i = 0; i < Contador; i++)
            {
                if (Contactos[i].Id == id)
                {
                    Console.Write("Nuevo nombre (dejalo vacío si no querés cambiarlo): ");
                    string nuevoNombre = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nuevoNombre))
                    {
                        Contactos[i].Nombre = nuevoNombre;
                    }

                    Console.Write("Nuevo teléfono (dejalo vacío si no querés cambiarlo): ");
                    string nuevoTelefono = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nuevoTelefono))
                    {
                        Contactos[i].Telefono = nuevoTelefono;
                    }

                    Console.Write("Nuevo email (dejalo vacío si no querés cambiarlo): ");
                    string nuevoEmail = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nuevoEmail))
                    {
                        Contactos[i].Email = nuevoEmail;
                    }

                    Console.WriteLine("¡Contacto modificado con éxito!");
                    return;
                }
            }
            Console.WriteLine("No encontré el contacto, fijate si el ID es correcto.");
        }
        else
        {
            Console.WriteLine("Ese ID no es válido, capo.");
        }
    }

    public void BorrarContacto()
    {
        Console.Write("Decime el ID del contacto que querés borrar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            for (int i = 0; i < Contador; i++)
            {
                if (Contactos[i].Id == id)
                {
                    for (int j = i; j < Contador - 1; j++)
                    {
                        Contactos[j] = Contactos[j + 1];
                    }
                    Contador--;
                    Console.WriteLine("¡Contacto borrado con éxito!");
                    return;
                }
            }
            Console.WriteLine("No encontré el contacto, fijate si el ID es correcto.");
        }
        else
        {
            Console.WriteLine("Ese ID no es válido.");
        }
    }
}

class Program
{
    static void Main()
    {
        Agenda agenda = new Agenda();
        agenda.CargarContactos();

        int opcion;
        do
        {
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contactos");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("0) Salir");
            Console.Write("Elegí una opción: ");

            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.Clear();
                switch (opcion)
                {
                    case 1: agenda.AgregarContacto(); break;
                    case 2: agenda.ModificarContacto(); break;
                    case 3: agenda.BorrarContacto(); break;
                    case 4: agenda.ListarContactos(); break;
                    case 5: agenda.BuscarContacto(); break;
                    case 0:
                        agenda.GuardarContactos();
                        Console.WriteLine("Saliendo, ¡nos vemos! Avisame a que hora juega Boca! :)");
                        break;
                    default:
                        Console.WriteLine("Esa opción no es válida, probá de nuevo.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Poné una opción válida, por favor.");
                opcion = -1;
            }

        } while (opcion != 0);
    }
}