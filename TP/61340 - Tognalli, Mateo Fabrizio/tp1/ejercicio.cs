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
Console.ReadKey();struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}
class Program
{
    const int MAX_CONTACTOS = 50;
    static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
    static int cantidadContactos = 0;
    static int ultimoId = 0;
    const string archivoCSV = "agenda.csv";

    static void Main()
    {
        CargarContactos(); 
        while (true)
        {
            MostrarMenu();
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
                    ListarContactos();
                    break;
                case "5":
                    BuscarContacto();
                    break;
                case "6":
                    GuardarContactos(); 
                    return; 
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("AGENDA DE CONTACTOS");
        Console.WriteLine("1. Agregar contacto");
        Console.WriteLine("2. Modificar contacto");
        Console.WriteLine("3. Borrar contacto");
        Console.WriteLine("4. Listar contactos");
        Console.WriteLine("5. Buscar contacto");
        Console.WriteLine("6. Salir");
        Console.Write("Seleccione una opción: ");
    }

    static void AgregarContacto()
    {
        if (cantidadContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("No se pueden agregar más contactos.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine().Trim();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine().Trim();
        Console.Write("Email: ");
        string email = Console.ReadLine().Trim();

        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Todos los campos son obligatorios.");
            return;
        }

        bool telefonoDuplicado = false;
        bool emailDuplicado = false;

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Telefono == telefono)
                telefonoDuplicado = true;
            if (agenda[i].Email == email)
                emailDuplicado = true;
        }

        if (telefonoDuplicado || emailDuplicado)
        {
            if (telefonoDuplicado) Console.WriteLine("El número de teléfono ya está registrado.");
            if (emailDuplicado) Console.WriteLine("El email ya está registrado.");
            return;
        }

        agenda[cantidadContactos] = new Contacto { Id = ++ultimoId, Nombre = nombre, Telefono = telefono, Email = email };
        cantidadContactos++;
        Console.WriteLine("Contacto agregado correctamente.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write("Nuevo Nombre (dejar vacío para no cambiar): ");
                string nombre = Console.ReadLine().Trim();
                Console.Write("Nuevo Teléfono (dejar vacío para no cambiar): ");
                string telefono = Console.ReadLine().Trim();
                Console.Write("Nuevo Email (dejar vacío para no cambiar): ");
                string email = Console.ReadLine().Trim();

                if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;
                if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;
                if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;

                Console.WriteLine("Contacto modificado correctamente.");
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

        int indice = -1;
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                indice = i;
                break;
            }
        }

        if (indice == -1)
        {
            Console.WriteLine("Contacto no encontrado.");
            return;
        }

        
        for (int j = indice; j < cantidadContactos - 1; j++)
        {
            agenda[j] = agenda[j + 1];
        }

        cantidadContactos--;

        
        for (int i = 0; i < cantidadContactos; i++)
        {
            agenda[i].Id = i + 1; 
        }

        ultimoId = cantidadContactos; 

        Console.WriteLine("Contacto eliminado correctamente.");
    }

    static void ListarContactos()
    {
        Console.Clear();
        if (cantidadContactos == 0)
        {
            Console.WriteLine("No hay contactos en la agenda.");
        }
        else
        {
            Console.WriteLine("\n-------------------------------------------------");
            Console.WriteLine("ID   Nombre              Teléfono       Email");
            Console.WriteLine("-------------------------------------------------");
            for (int i = 0; i < cantidadContactos; i++)
            {
                Console.WriteLine($"{agenda[i].Id,-4} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
            }
            Console.WriteLine("-------------------------------------------------");
        }
        Console.WriteLine("\nPresione cualquier tecla para volver al menú.");
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        bool encontrado = false;

        Console.WriteLine("\nID   Nombre              Teléfono       Email");
        Console.WriteLine("-------------------------------------------------");
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-4} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron contactos con ese término.");
        }
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("\nPresione cualquier tecla para volver al menú.");
        Console.ReadKey();
    }

    static void CargarContactos()
    {
        if (File.Exists(archivoCSV))
        {
            try
            {
                string[] lineas = File.ReadAllLines(archivoCSV);
                if (lineas.Length > 0)
                {
                    
                    for (int i = 1; i < lineas.Length; i++)
                    {
                        string[] datos = lineas[i].Split(',');
                        if (datos.Length == 3)
                        {
                            agenda[cantidadContactos] = new Contacto
                            {
                                Id = ++ultimoId,
                                Nombre = datos[0],
                                Telefono = datos[1],
                                Email = datos[2]
                            };
                            cantidadContactos++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo CSV: {ex.Message}");
            }
        }
    }

    static void GuardarContactos()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(archivoCSV))
            {
                
                sw.WriteLine("Nombre,Telefono,Email");

                
                for (int i = 0; i < cantidadContactos; i++)
                {
                    sw.WriteLine($"{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
                }
            }
            Console.WriteLine("Los cambios han sido guardados.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo CSV: {ex.Message}");
        }
    }
}