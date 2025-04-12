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
    static Contacto[] contactos = new Contacto[MaxContactos];
    static int contadorContactos = 0;
    static string archivoAgenda = "agenda.csv";
    static void Main()
    {
        CargarContactosDesdeArchivo();

        while (true)
        {
            Clear();
            WriteLine("----------Agenda de contactos----------");
            WriteLine("-1) Agregar contacto                  -");
            WriteLine(" 2) Modificar contacto                -");
            WriteLine("-3) Borrar contacto                   -");
            WriteLine("-4) Listar contactos                  -");  
            WriteLine("-5) Buscar contacto                   -");
            WriteLine("-6) Salir                             -");
            WriteLine("---------------------------------------");
            Write("Seleccione una opción: ");
            string opcion = ReadLine();
            if (opcion == "1")
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
                ListarContactos();
            }
            else if (opcion == "5")
            {
                BuscarContacto();
            }
            else if (opcion == "6")
            {
                GuardarContactosEnArchivo();
                break;
            }
            else
            {
                WriteLine("Opción no válida. Presione una tecla para continuar...");
                ReadKey();
            }
        }
    }
    static void CargarContactosDesdeArchivo()
    {
        if (File.Exists(archivoAgenda))
        {
            string[] lineas = File.ReadAllLines(archivoAgenda);
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split(',');
                if (datos.Length == 4 && contadorContactos < MaxContactos)
                {
                    contactos[contadorContactos++] = new Contacto
                    {
                        Id = int.Parse(datos[0]),
                        Nombre = datos[1],
                        Telefono = datos[2],
                        Email = datos[3]
                    };
                }
            }
        }
    }
    static void GuardarContactosEnArchivo()
    {
    var lineas = new List<string>();
    for (int i = 0; i < contadorContactos; i++)
    {
        lineas.Add($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
    }
    File.WriteAllLines(archivoAgenda, lineas);
    }
    static void AgregarContacto()
    {
        if (contadorContactos >= MaxContactos)
        {
            WriteLine("La agenda está llena.");
            ReadKey();
            return;
        }
        WriteLine("-----Agregar contacto-----");
        Write("-Nombre   : ");
        string nombre = ReadLine();
        Write("-Teléfono : ");
        string telefono = ReadLine();
        Write("-Email    : ");
        string email = ReadLine();
        contactos[contadorContactos] = new Contacto
        {
            Id = contadorContactos + 1,
            Nombre = nombre,
            Telefono = telefono,
            Email = email
        };
        contadorContactos++;

        WriteLine($"Contacto agregado con ID = {contadorContactos}");
        WriteLine("Presione una tecla para volver al menú principal.");
        ReadKey();
    }
    static void ModificarContacto()
    {
        WriteLine("-----Modificar Contacto-----");
        Write("Ingrese el ID del contacto a modificar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                if (contactos[i].Id == id)
                {
                    WriteLine($"Datos actuales => Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
                    Write("(Deje espacio en blanco para no modificar.)\n");
                    Write("Nombre    : ");
                    string nombre = ReadLine();
                    Write("Teléfono  : ");
                    string telefono = ReadLine();
                    Write("Email     : ");
                    string email = ReadLine();

                    if (!string.IsNullOrWhiteSpace(nombre)) contactos[i].Nombre = nombre;
                    if (!string.IsNullOrWhiteSpace(telefono)) contactos[i].Telefono = telefono;
                    if (!string.IsNullOrWhiteSpace(email)) contactos[i].Email = email;

                    WriteLine("Contacto modificado con éxitosamente.");
                    ReadKey();
                    return;
                }
            }
        }
        WriteLine("ID no encontrado.");
        WriteLine("Presione una tecla para volver al menú principal.");
        ReadKey();
    }
    static void BorrarContacto()
    {
        WriteLine("-----Borrar Contacto-----");
        Write("Ingrese el ID del contacto que desea borrar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                if (contactos[i].Id == id)
                {
                    for (int j = i; j < contadorContactos - 1; j++)
                    {
                        contactos[j] = contactos[j + 1];
                    }
                    contadorContactos--;
                    WriteLine($"Contacto con ID={id} eliminado con éxito.");
                    ReadKey();
                    return;
                }
            }
        }
        WriteLine("ID no encontrado.");
        WriteLine("Presione una tecla para volver al menú principal.");
        ReadKey();
    }
    static void ListarContactos()
    {
        WriteLine("-----Lista de Contactos-----");
        WriteLine("-----------------------------------------------");
        WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < contadorContactos; i++)
        {
            WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-25}");
        }
        ReadKey();
        WriteLine("Presione una tecla para volver al menú principal.");
    }
    static void BuscarContacto()
    {
        WriteLine("-----Buscar Contacto-----");
        Write("Ingrese un término de búsqueda (id, nombre, teléfono o email): ");
        string termino = ReadLine().ToLower();
        WriteLine("Resultados de la búsqueda:");
        WriteLine("-----------------------------------------------");
        WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < contadorContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-25}");
            }
        }
        WriteLine("Presione una tecla para volver al menú principal.");
        ReadKey();
    }
}