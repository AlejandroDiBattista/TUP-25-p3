using System;
using System.IO;
using static System.Console;

class Program
{
    struct Contacto  
    {
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Email;
    }

    static Contacto[] Contactos = new Contacto[50]; 
    static int totalContactos = 0;
    static string archivoCSV = "agenda.csv"; 

    static void Main()
    {
        CargarContactos(); 

        while (true)
        {
            Clear();
           WriteLine("=== AGENDA DE CONTACTOS ===");
            WriteLine("1. Agregar Contacto");
            WriteLine("2. Modificar Contacto");
            WriteLine("3. Borrar Contacto");
            WriteLine("4. Listar Contactos");
            WriteLine("5. Buscar Contacto");
            WriteLine("0. Salir");
            Write("Seleccione una opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "0": GuardarContactos(); return; 
                default:
                    WriteLine("Opción no válida. Presione una tecla para continuar...");
                    ReadKey();
                    break;
            }
        }
    }

    static void CargarContactos()
    {
        if (File.Exists(archivoCSV))
        {
            string[] lineas = File.ReadAllLines(archivoCSV);
            for (int i = 0; i < lineas.Length; i++) 
            {
                var datos = lineas[i].Split(',');

                if (datos.Length == 4) 
                {
                    Contacto nuevoContacto;
                    nuevoContacto.Id = int.Parse(datos[0]);
                    nuevoContacto.Nombre = datos[1];
                    nuevoContacto.Telefono = datos[2];
                    nuevoContacto.Email = datos[3];

                    Contactos[totalContactos] = nuevoContacto;
                    totalContactos++;
                }
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivoCSV))
        {
            for (int i = 0; i < totalContactos; i++) 
            {
                if (!string.IsNullOrEmpty(Contactos[i].Nombre)) 
                {
                    sw.WriteLine($"{Contactos[i].Id},{Contactos[i].Nombre},{Contactos[i].Telefono},{Contactos[i].Email}");
                }
            }
        }
        WriteLine("Contactos guardados exitosamente.");
    }

    static void AgregarContacto()
    {
        if (totalContactos >= Contactos.Length)
        {
            WriteLine("Agenda llena. No se pueden agregar más contactos.");
           ReadKey();
            return;
        }

        Contacto nuevoContacto;
        nuevoContacto.Id = totalContactos + 1; 

        Write("Ingrese el nombre del contacto: ");
        nuevoContacto.Nombre = ReadLine();

        Write("Ingrese el teléfono del contacto: ");
        nuevoContacto.Telefono = ReadLine();

        Write("Ingrese el email del contacto: ");
        nuevoContacto.Email = ReadLine();

        Contactos[totalContactos] = nuevoContacto; 
        totalContactos++;

        WriteLine("Contacto agregado con éxito.");
        ReadKey();
    }

    static void ListarContactos()
    {
        WriteLine("\n=== Lista de Contactos === :");
        for (int i = 0; i < totalContactos; i++)
        {
            WriteLine($"ID: {Contactos[i].Id} | Nombre: {Contactos[i].Nombre} | Teléfono: {Contactos[i].Telefono} | Email: {Contactos[i].Email}");
        }
       ReadKey();
    }

    static void BuscarContacto()
    {
        Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
        string nombreBuscado = ReadLine();

        bool encontrado = false;
        for (int i = 0; i < totalContactos; i++)
        {
            if (Contactos[i].Nombre.ToLower() == nombreBuscado.ToLower())
            {
                WriteLine($"\nContacto encontrado:\nID: {Contactos[i].Id} | Nombre: {Contactos[i].Nombre} | Teléfono: {Contactos[i].Telefono} | Email: {Contactos[i].Email}");
                encontrado = true;
                break;
            }
        }

        if (!encontrado)
            WriteLine("Contacto no encontrado.");

        ReadKey();
    }

    static void ModificarContacto()
    {
        Write("Ingrese el ID del contacto a modificar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            for (int i = 0; i < totalContactos; i++)
            {
                if (Contactos[i].Id == id)
                {
                    Console.Write("Nuevo nombre: ");
                    Contactos[i].Nombre = ReadLine();

                    Console.Write("Nuevo teléfono: ");
                    Contactos[i].Telefono = ReadLine();

                    Console.Write("Nuevo email: ");
                    Contactos[i].Email = ReadLine();

                    WriteLine("Contacto modificado con éxito.");
                    ReadKey();
                    return;
                }
            }
            WriteLine("ID no encontrado.");
        }
        else
        {
            WriteLine("ID inválido.");
        }
        ReadKey();
    }

    static void BorrarContacto()
    {
        Write("Ingrese el ID del contacto a borrar: ");
        if (int.TryParse(ReadLine(), out int id))
        {
            for (int i = 0; i < totalContactos; i++)
            {
                if (Contactos[i].Id == id)
                {
                    for (int j = i; j < totalContactos - 1; j++)
                    {
                        Contactos[j] = Contactos[j + 1];
                    }
                    totalContactos--;
                    WriteLine("Contacto eliminado.");
                    ReadKey();
                    return;
                }
            }
            WriteLine("ID no encontrado.");
        }
        else
        {
            WriteLine("ID inválido.");
        }
       ReadKey();
    }
}
