/*
    Profe si no ve muchos cambios realizados es porqué primero los hice pero puse el nombre de
    rama al revés, y no sabía si se podía cambiar el nombre de la rama, así que hice una nueva
    y subí los cambios ahí.
*/

using System;       // Para usar la consola
using System.IO;    // Para leer archivos

struct Contacto {
    private static int contadorId = 1;
    public int Id { get; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }

    // Constructor que asigna automáticamente un ID único al contacto
    public Contacto(string nombre, string telefono, string email)
    {
        Id = contadorId++;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }

    // Método para mostrar la información del contacto
    public void MostrarInfo()
    {
        Console.WriteLine($"ID: {Id} | Nombre: {Nombre} | Teléfono: {Telefono} | Email: {Email}");
    }
}

class Agenda {
    private int cantidadContactos;
    private Contacto[] contactos;
    private int maxContactos;
    private string archivoContactos = "agenda.csv";

    public Agenda(int capacidadInicial = 100)
    {
        cantidadContactos = 0;
        maxContactos = capacidadInicial;
        contactos = new Contacto[maxContactos];
        CargarContactosDesdeArchivo();
    }

    public void AgregarContacto()
    {
        if (cantidadContactos >= maxContactos)
        {
            Console.WriteLine("Agenda llena. Aumentando capacidad...");
            AumentarCapacidad();
        }

        Console.Clear();
        Console.WriteLine("=== Agregar Contacto ===");

        string nombre, telefono, email;

        do
        {
            Console.Write("Nombre: ");
            nombre = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                Console.WriteLine("====================================================================");
                Console.WriteLine("El nombre no puede estar vacío. Por favor, ingresá un nombre válido.");
                Console.WriteLine("====================================================================");
            }
        } while (string.IsNullOrEmpty(nombre));

        do
        {
            Console.Write("Teléfono: ");
            telefono = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(telefono))
            {
                Console.WriteLine("==================================================================================");
                Console.WriteLine("El teléfono no puede estar vacío. Por favor, ingresá un número de teléfono válido.");
                Console.WriteLine("==================================================================================");
            }
        } while (string.IsNullOrEmpty(telefono));

        do
        {
            Console.Write("Email: ");
            email = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                Console.WriteLine("============================================================");
                Console.WriteLine("El email no es válido. Por favor, ingresá un email correcto.");
                Console.WriteLine("============================================================");
            }
        } while (string.IsNullOrEmpty(email) || !email.Contains("@"));


        Contacto nuevoContacto = new Contacto(nombre, telefono, email);
        contactos[cantidadContactos] = nuevoContacto;
        cantidadContactos++;

        Console.WriteLine("====================================================");
        Console.WriteLine($"El contacto: {nombre}, fue agregado correctamente con ID: {contactos[cantidadContactos - 1].Id}.");
        Console.WriteLine("====================================================");
    }

    public void ModificarContacto()
    {
        Console.Clear();

        if (cantidadContactos == 0)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("No hay contactos en la agenda.");
            Console.WriteLine("===================================");
            return;
        }

        Console.WriteLine("=== Modificar Contacto ===");

        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            int index = BuscarIndicePorId(id);
            if (index != -1)
            {
                Console.WriteLine("========================================================");
                Console.WriteLine($"Datos actuales => Nombre: {contactos[index].Nombre}, Teléfono: {contactos[index].Telefono}, Email: {contactos[index]}.");
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("Si no deseás modificar un campo, dejálo vacío.");
                Console.WriteLine("========================================================");

                Console.Write("Ingresá el nuevo nombre: ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre))
                {
                    contactos[index].Nombre = nuevoNombre;
                }

                Console.Write("Ingresá el nuevo teléfono: ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoTelefono))
                {
                    contactos[index].Telefono = nuevoTelefono;
                }

                Console.Write("Ingresá el nuevo email: ");
                string nuevoEmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoEmail))
                {
                    contactos[index].Email = nuevoEmail;
                }

                Console.WriteLine("========================================================");
                Console.WriteLine($"El contacto con ID: {id} fue modificado correctamente.");
                Console.WriteLine("========================================================");
            }
            else
            {
                Console.WriteLine("==========================");
                Console.WriteLine("Contacto no encontrado.");
                Console.WriteLine("==========================");
            }
        }
        else
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("ID inválido. Por favor, ingresá un número.");
            Console.WriteLine("===========================================");
        }
    }

    public void BorrarContacto()
    {
        Console.Clear();

        if (cantidadContactos == 0)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("No hay contactos en la agenda.");
            Console.WriteLine("===================================");
            return;
        }

        Console.WriteLine("=== Borrar Contacto ===");

        Console.Write("Ingresá el ID del contacto a borrar: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            int index = BuscarIndicePorId(id);
            if (index != -1)
            {
                for (int i = index; i < cantidadContactos - 1; i++)
                {
                    contactos[i] = contactos[i + 1];
                }
                contactos[cantidadContactos - 1] = default(Contacto); // Para limpiar la última posición
                cantidadContactos--;
                Console.WriteLine("================================================");
                Console.WriteLine($"El contacto con ID: {id} fue borrado correctamente.");
                Console.WriteLine("================================================");
            }
            else
            {
                Console.WriteLine("=======================");
                Console.WriteLine("Contacto no encontrado.");
                Console.WriteLine("=======================");
            }
        }
        else
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("ID inválido. Por favor, ingresá un número.");
            Console.WriteLine("==========================================");
        }
    }

    public void ListarContactos()
    {
        Console.Clear();

        if (cantidadContactos == 0)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("No hay contactos en la agenda.");
            Console.WriteLine("===================================");
            return;
        }

        Console.WriteLine("=== Lista de Contactos ===");
        Console.WriteLine($"{ "ID",-4} { "Nombre",-20} { "Teléfono",-12} { "Email",-30}");
        
        for (int i = 0; i < cantidadContactos; i++)
        {
            Console.WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-20} {contactos[i].Telefono,-12} {contactos[i].Email,-30}");
        }
    }

    public void BuscarContacto()
    {
        Console.Clear();

        if (cantidadContactos == 0)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("No hay contactos en la agenda.");
            Console.WriteLine("===================================");
            return;
        }

        Console.Write("Ingresá un término de búsqueda(nombre, teléfono o email): ");
        string busqueda = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrEmpty(busqueda))
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("La búsqueda no puede estar vacía. Por favor, ingresá un término.");
            Console.WriteLine("=====================================================");
            return;
        }

        // Se filtranado los contactos que coinciden con la búsqueda
        var resultados = contactos
            .Where(c => !string.IsNullOrEmpty(c.Nombre) &&
                (c.Nombre.ToLower().Contains(busqueda) ||
                c.Telefono.ToLower().Contains(busqueda) ||
                c.Email.ToLower().Contains(busqueda))
            ).ToArray();

        if (resultados.Length == 0)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("No se encontraron contactos que coincidan con la búsqueda.");
            Console.WriteLine("===================================");
        }

        Console.WriteLine("===================================");
        Console.WriteLine("Resultados de la búsqueda:");
        Console.WriteLine($"{ "ID",-4} { "Nombre",-20} { "Teléfono",-12} { "Email",-30}");

        for (int i = 0; i < resultados.Length; i++)
        {
            Console.WriteLine($"{resultados[i].Id,-4} {resultados[i].Nombre,-20} {resultados[i].Telefono,-12} {resultados[i].Email,-30}");
        } 
    }

    private int BuscarIndicePorId(int id)
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                return i;
            }
        }
        return -1; // No se encontró el contacto
    }

    private void AumentarCapacidad()
    {
        int nuevaCapacidad = maxContactos + 50;
        Contacto[] nuevosContactos = new Contacto[nuevaCapacidad];

        for ( int i = 0; i < cantidadContactos; i++)
        {
            nuevosContactos[i] = contactos[i];
        }

        contactos = nuevosContactos;
        maxContactos = nuevaCapacidad;
    }

    private void CargarContactosDesdeArchivo()
    {
        if (File.Exists(archivoContactos))
        {
            using (StreamReader sr = new StreamReader(archivoContactos))
            {
                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string[] datos = linea.Split(',');
                    if (datos.Length == 4 && int.TryParse(datos[0], out int id))
                    {
                        Contacto contacto = new Contacto(datos[1], datos[2], datos[3]);
                        contactos[cantidadContactos] = contacto;
                        cantidadContactos++;
                    }
                }
            }
        }
    }

    public void GuardarContactosEnArchivo()
    {
        using (StreamWriter sw = new StreamWriter(archivoContactos, false))
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
    }
}

int opcion;

Agenda agenda = new Agenda();

do
{
    Console.Clear();
    Console.WriteLine("===== AGENDA DE CONTACTOS =====");
    Console.WriteLine("1) Agregar contacto");
    Console.WriteLine("2) Modificar contacto");
    Console.WriteLine("3) Borrar contacto");
    Console.WriteLine("4) Listar contactos");
    Console.WriteLine("5) Buscar contacto");
    Console.WriteLine("0) Salir");
    Console.Write("Seleccioná una opción: ");

    string entrada = Console.ReadLine();
    
    if (int.TryParse(entrada, out opcion))
    {
        switch (opcion)
        {
            case 1:
                agenda.AgregarContacto();
                break;
            case 2:
                agenda.ModificarContacto();
                break;
            case 3:
                agenda.BorrarContacto();
                break;
            case 4:
                agenda.ListarContactos();
                break;
            case 5:
                agenda.BuscarContacto();
                break;
            case 0:
                agenda.GuardarContactosEnArchivo();
                Console.WriteLine("===================================");
                Console.WriteLine("Guardando contactos en el archivo...");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Saliendo...");
                Console.WriteLine("===================================");
                break;
            default:
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("Opción inválida");
                Console.WriteLine("===================================");
                break;
        }
    }
    else
    {
        Console.WriteLine("Por favor, ingresá un número");
        opcion = -1;
    }
    Console.WriteLine("Presioná una tecla para continuar...");
    Console.ReadKey();
} while (opcion != 0);