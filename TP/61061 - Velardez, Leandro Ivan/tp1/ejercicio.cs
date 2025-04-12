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

using static System.Console;
using System.IO;


struct contacto
{
    public string Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}
public class Program 
{
    const int MAX = 100;
    static contacto[] contactos = new contacto[MAX];
    static int totalContactos = 0;
    static int nextId = 1;
    static string archivo = Path.Combine(Directory.GetCurrentDirectory(), "agenda.csv");


    public static void Main()
    {
        
        WriteLine ($"Directorio actual: {Directory.GetCurrentDirectory()}");
        CargarContactos();
        while (true)
        {
            WriteLine("\n===== AGENDA DE CONTACTOS =====");
            WriteLine("1) Agregar contacto");
            WriteLine("2) Modificar contacto");
            WriteLine("3) Borrar contacto");
            WriteLine("4) Listar contactos");
            WriteLine("5) Buscar contacto");
            WriteLine("0) Salir");
            Write("Seleccione una opción: ");

            string opcion = ReadLine() ?? "0";

            if (opcion == "0")
            {
                GuardarContactos();
                break;
            }
            else if (opcion == "1")
            {
                AgregarContacto();
            }
            else if (opcion == "2")
            {
                ModificarContacto();
            }
            else if (opcion == "3")
            {
                BorrarContacto();
            }
            else if (opcion == "4")
            {
                MostrarContactos();
            }
            else if (opcion == "5")
            {
                BuscarContacto();
            }
            else
            {
                WriteLine("Opción no válida. Intente de nuevo.");
            }
        }
    }

    static void GuardarContactos()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(archivo, false))
            {
                for (int i = 0; i < totalContactos; i++)
                {
                    sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
                }
            }
            // WriteLine("Contactos guardados exitosamente.");
        }
        catch (Exception ex)
        {
            WriteLine($"Error al guardar los contactos: {ex.Message}\n{ex.StackTrace}");
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivo))
        {
            WriteLine("No se encontró el archivo de contactos. Se creará uno nuevo al guardar.");
            return;
        }

        try
        {
            using (StreamReader reader = new StreamReader(archivo))
            {
                string linea;
                while ((linea = reader.ReadLine()) != null)
                {
                    string[] datos = linea.Split(',');

                    if (datos.Length == 4)
                    {
                        contactos[totalContactos] = new contacto
                        {
                            Id = datos[0],
                            Nombre = datos[1],
                            Telefono = datos[2],
                            Email = datos[3]
                        };
                        totalContactos++;
                        nextId = Math.Max(nextId, int.Parse(datos[0]) + 1);
                    }
                }
            }
            WriteLine("Contactos cargados exitosamente.");
        }
        catch (Exception ex)
        {
            WriteLine($"Error al cargar los contactos: {ex.Message}\n{ex.StackTrace}");
        }
    }

    static void AgregarContacto()
    {
        Clear();

        WriteLine("=== Agregar Contacto ===");
        if (totalContactos >= MAX)
        {
            WriteLine("No se pueden agregar más contactos.");
            return;
        }
        WriteLine("Nombre : ");
        string nombre = ReadLine();

            if (string.IsNullOrEmpty(nombre)) {
        WriteLine("El nombre no puede estar vacío.");
        return;
            } 

        WriteLine("Teléfono :");
        string telefono = ReadLine();

            if (string.IsNullOrEmpty(telefono)) {
        WriteLine("El teléfono no puede estar vacío.");
        return;
            }


        WriteLine("Email :");
        string email = ReadLine();

            if (string.IsNullOrEmpty(email)) {
        WriteLine("El email no puede estar vacío.");
        return;
            }   
        
        // GuardarContactos();

        // WriteLine("Contacto agregado con ID :");
        // string Id = ReadLine();

        contactos[totalContactos] = new contacto
        {
            Id = (nextId++).ToString(),
            Nombre = nombre,
            Telefono = telefono,
            Email = email
        };
        totalContactos++;
        // nextId++;

        GuardarContactos();
        
        WriteLine();
        WriteLine("=== Contacto Agregado ===");
        WriteLine($"Nombre: {nombre}");
        WriteLine($"Teléfono: {telefono}");
        WriteLine($"Email: {email}");
        WriteLine($"Contacto agregado con ID = {contactos[totalContactos - 1].Id}");

        // WriteLine("Contacto agregado exitosamente.");
        ReadKey();
        Clear();
    }

    static void MostrarContactos()
    {
        Clear();
        WriteLine("=== Lista de Contactos ===");
        if (totalContactos == 0)
        {
            WriteLine("No hay contactos para mostrar.");
            return;
        }

            WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");
            WriteLine(new string('-', 70));


        for (int i = 0; i < totalContactos; i++)
            {
                WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            }

        // for (int i = 0; i < totalContactos; i++)
        // {
        //     WriteLine($"ID: {contactos[i].Id}, Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
        // }
        ReadKey();
    }

    static void BuscarContacto()
    {
        WriteLine("=== Buscar Contacto ===");
        WriteLine("Ingrese el término de búsqueda (nombre, teléfono o correo):");
        string termino = ReadLine()?.ToLower();

        bool encontrado = false;
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                WriteLine($"ID: {contactos[i].Id}, Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
                encontrado = true;
            }
        }
        if (!encontrado)
        {
            WriteLine("No se encontró ningún contacto con ese término.");
        }
    }

    static void ModificarContacto()
    {
        WriteLine("=== Modificar Contacto ===");
        WriteLine("Ingrese el ID del contacto a modificar:");
        string id = ReadLine();

        bool encontrado = false;
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                encontrado = true;
                WriteLine($"Datos actuales => Nombre: {contactos[i].Nombre}, Teléfono : {contactos[i].Telefono}, Email: {contactos[i].Email}");
                WriteLine("Deje el campo en blanco para no modoficar ");

                WriteLine("Nombre):");
                string nuevoNombre = ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre))
                {
                    contactos[i].Nombre = nuevoNombre;
                }

                WriteLine("Telefono):");
                string nuevoTelefono = ReadLine();
                if (!string.IsNullOrEmpty(nuevoTelefono))
                {
                    contactos[i].Telefono = nuevoTelefono;
                }

                WriteLine("Email):");
                string nuevoEmail = ReadLine();
                if (!string.IsNullOrEmpty(nuevoEmail))
                {
                    contactos[i].Email = nuevoEmail;
                }

                GuardarContactos(); 

                WriteLine("Contacto modificado exitosamente.");
                break;
            }
        }

        if (!encontrado)
        {
            WriteLine("No se encontró ningún contacto con ese ID.");
        }
        ReadKey();
    }
     static void BorrarContacto()
    {
        WriteLine("=== Borrar Contacto ===");
        WriteLine("Ingrese el ID del contacto a borrar:");
        string id = ReadLine();

        bool encontrado = false;
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                encontrado = true;
                WriteLine($"Contacto encontrado: ID: {contactos[i].Id}, Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
                WriteLine("¿Está seguro de que desea eliminar este contacto? (s/n):");
                string confirmacion = ReadLine()?.ToLower();

                if (confirmacion == "s")
                {
                    for (int j = i; j < totalContactos - 1; j++)
                    {
                        contactos[j] = contactos[j + 1];
                    }
                    totalContactos--;

                    GuardarContactos(); 
                    WriteLine("Contacto eliminado exitosamente.");
                }
                else
                {
                    WriteLine("Operación cancelada.");
                }
                break;
            }
        }
        if (!encontrado)
        {
            WriteLine("No se encontró ningún contacto con ese ID.");
        }
        ReadKey();
    }
}
