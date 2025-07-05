using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Agenda
{
    static void Main(string[] args)
    {
        const int MAX_CONTACTOS = 100;
        Contacto[] agenda = new Contacto[MAX_CONTACTOS];
        int contadorContactos = 0;

        
        CargarContactosDesdeArchivo(agenda, ref contadorContactos);

        while (true)
        {
            Console.WriteLine("\nMenú:");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");
            
            string opcion = Console.ReadLine();

            if (opcion == "1")
                AgregarContacto(agenda, ref contadorContactos);
            else if (opcion == "2")
                ModificarContacto(agenda, contadorContactos);
            else if (opcion == "3")
                BorrarContacto(agenda, ref contadorContactos);
            else if (opcion == "4")
                ListarContactos(agenda, contadorContactos);
            else if (opcion == "5")
                BuscarContacto(agenda, contadorContactos);
            else if (opcion == "6")
                break;
            else
                Console.WriteLine("Opción no válida, intente nuevamente.");
        }

        
        GuardarContactosEnArchivo(agenda, contadorContactos);
    }

    static void CargarContactosDesdeArchivo(Contacto[] agenda, ref int contador)
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] partes = lineas[i].Split(',');
                if (partes.Length == 4)
                {
                    agenda[contador].Id = int.Parse(partes[0]);
                    agenda[contador].Nombre = partes[1];
                    agenda[contador].Telefono = partes[2];
                    agenda[contador].Email = partes[3];
                    contador++;
                }
            }
        }
    }

    static void AgregarContacto(Contacto[] agenda, ref int contador)
    {
        if (contador >= agenda.Length)
        {
            Console.WriteLine("No se pueden agregar más contactos.");
            return;
        }

        Contacto nuevoContacto;
        nuevoContacto.Id = contador + 1;

        Console.Write("Ingrese nombre: ");
        nuevoContacto.Nombre = Console.ReadLine();
        
        Console.Write("Ingrese teléfono: ");
        nuevoContacto.Telefono = Console.ReadLine();
        
        Console.Write("Ingrese email: ");
        nuevoContacto.Email = Console.ReadLine();

        agenda[contador] = nuevoContacto;
        contador++;
        
        Console.WriteLine("Contacto agregado exitosamente.");
    }

    static void ModificarContacto(Contacto[] agenda, int contador)
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int idModificar = int.Parse(Console.ReadLine());

        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == idModificar)
            {
                Console.Write($"Nuevo nombre (actual: {agenda[i].Nombre}): ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nuevoNombre)) 
                    agenda[i].Nombre = nuevoNombre;

                Console.Write($"Nuevo teléfono (actual: {agenda[i].Telefono}): ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nuevoTelefono)) 
                    agenda[i].Telefono = nuevoTelefono;

                Console.Write($"Nuevo email (actual: {agenda[i].Email}): ");
                string nuevoEmail = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nuevoEmail)) 
                    agenda[i].Email = nuevoEmail;

                Console.WriteLine("Contacto modificado exitosamente.");
                return;
            }
        }

        Console.WriteLine("No se encontró ningún contacto con ese ID.");
    }

    static void BorrarContacto(Contacto[] agenda, ref int contador)
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int idBorrar = int.Parse(Console.ReadLine());

        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == idBorrar)
            {
                
                for (int j = i; j < contador - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                
                contador--;
                Console.WriteLine("Contacto borrado exitosamente.");
                return;
            }
        }

        Console.WriteLine("No se encontró ningún contacto con ese ID.");
    }

    static void ListarContactos(Contacto[] agenda, int contador)
    {
        Console.WriteLine("\nListado de Contactos:");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Teléfono", "Email");
        
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", 
                              agenda[i].Id, 
                              agenda[i].Nombre, 
                              agenda[i].Telefono, 
                              agenda[i].Email);
        }
    }

    static void BuscarContacto(Contacto[] agenda, int contador)
    {
        Console.Write("Ingrese término de búsqueda: ");
        string terminoBusqueda = Console.ReadLine().ToLower();

        bool encontrado = false;
        
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(terminoBusqueda) ||
                agenda[i].Telefono.ToLower().Contains(terminoBusqueda) ||
                agenda[i].Email.ToLower().Contains(terminoBusqueda))
            {
                encontrado = true;
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", 
                                  agenda[i].Id,
                                  agenda[i].Nombre,
                                  agenda[i].Telefono,
                                  agenda[i].Email);
            }
        }
        
        if (!encontrado)
            Console.WriteLine("No se encontraron contactos que coincidan con la búsqueda.");
    }

    static void GuardarContactosEnArchivo(Contacto[] agenda, int contador)
    {
        using (StreamWriter writer = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < contador; i++)
            {
                writer.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
            
            writer.Flush();
            writer.Close();
            
            Console.WriteLine("Contactos guardados correctamente en el archivo.");
       }
   }
}