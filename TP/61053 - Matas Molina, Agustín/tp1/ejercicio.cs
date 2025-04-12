using System;       
using System.IO;    

public struct Contacto
{
    public int Id;
    public string Nombre; 
    public string Telefono; 
    public string Email;

    public Contacto(int id, string nombre, string telefono, string email)
    {
        this.Id = id;
        this.Nombre = nombre;
        this.Telefono = telefono;
        this.Email = email;
    }
}

Console.WriteLine("Bienvenido a la agenda de contactos");

funciones.cargarcontactos();

bool salir = false;
while (!salir)
{
    Console.WriteLine("1. Agregar contacto");
    Console.WriteLine("2. Modificar contacto");
    Console.WriteLine("3. Borrar contacto");
    Console.WriteLine("4. Listar contactos");
    Console.WriteLine("5. Buscar contacto");
    Console.WriteLine("6. Salir");
    Console.WriteLine("Ingrese una opcion");

    int opcion = int.Parse(Console.ReadLine());
    switch (opcion)
    {
        case 1:
            funciones.agregarcontacto();
            break;
        case 2:
            funciones.modificarcontacto();
            break;
        case 3:
            funciones.borrarcontacto();
            break;
        case 4:
            funciones.listarcontacto();
            break;
        case 5:
            funciones.buscarcontacto();
            break;
        case 6:
            
            funciones.guardarcontacto();
            salir = true;
            break;
        default:
            Console.WriteLine("Opcion no valida elija una de las opciones disponibles");
            break;
    }
}

public class funciones
{  
    private static Contacto[] contactos = new Contacto[100];
    private static int contador = 0;

   
    public static void cargarcontactos()
    {
        if (File.Exists("agenda.csv")) 
        {
            using (StreamReader reader = new StreamReader("agenda.csv"))
            {
                string linea;
                while ((linea = reader.ReadLine()) != null) 
                {
                    string[] datos = linea.Split(','); 
                    if (datos.Length == 4) 
                    {
                        int id = int.Parse(datos[0]);
                        string nombre = datos[1];
                        string telefono = datos[2];
                        string email = datos[3];

                        
                        contactos[contador] = new Contacto(id, nombre, telefono, email);
                        contador++;
                    }
                }
            }
            Console.WriteLine("Contactos cargados exitosamente desde agenda.csv.");
        }
        else
        {
            Console.WriteLine("No se encontró el archivo agenda.csv. Se iniciará con una agenda vacía.");
        }
    }

    public static void agregarcontacto()
    {
        if (contador >= contactos.Length)
        {
            Console.WriteLine("No se pueden agregar más contactos. Agenda llena.");
            return;
        }
        Contacto nuevoContacto = new Contacto(contador + 1, "", "", "");
        Console.WriteLine("============================================");
        Console.WriteLine("Ingrese el nombre del contacto:");
        nuevoContacto.Nombre = Console.ReadLine();
        Console.WriteLine("Ingrese el telefono del contacto:");
        nuevoContacto.Telefono = Console.ReadLine();
        Console.WriteLine("Ingrese el email del contacto:");
        nuevoContacto.Email = Console.ReadLine();
        
        contactos[contador] = nuevoContacto;
        contador++;
        Console.WriteLine("Contacto agregado con éxito.");
        Console.WriteLine("ID: " + nuevoContacto.Id);
    }

    public static void modificarcontacto()
    {    
        int id = -1; 
        bool idValido = false;

        while (!idValido)
        {
            Console.WriteLine("Ingrese el ID del contacto a modificar:");
            if (int.TryParse(Console.ReadLine(), out id) && id >= 1 && id <= contador)
            {
                idValido = true; 
            }
            else
            {
                Console.WriteLine("ID no válido. Intente nuevamente.");
            }
        }
        Contacto contacto = contactos[id - 1];
        Console.WriteLine("Modificar contacto: " + contacto.Nombre);
        Console.WriteLine("Ingrese nuevo nombre (dejar vacío para no cambiar):");
        string nuevoNombre = Console.ReadLine();
        if (!string.IsNullOrEmpty(nuevoNombre))
            contacto.Nombre = nuevoNombre;
        
        Console.WriteLine("Ingrese nuevo teléfono (dejar vacío para no cambiar):");
        string nuevoTelefono = Console.ReadLine();
        if (!string.IsNullOrEmpty(nuevoTelefono))
            contacto.Telefono = nuevoTelefono;
        
        Console.WriteLine("Ingrese nuevo email (dejar vacío para no cambiar):");
        string nuevoEmail = Console.ReadLine();
        if (!string.IsNullOrEmpty(nuevoEmail))
            contacto.Email = nuevoEmail;
        
        contactos[id - 1] = contacto;
        Console.WriteLine("Contacto modificado con éxito.");
    }

    public static void borrarcontacto()
    { 
        Console.WriteLine("Ingrese el ID del contacto a borrar:");
        int id = int.Parse(Console.ReadLine());
        if (id < 1 || id > contador)
        {
            Console.WriteLine("ID no válido.");
            return;
        }
        for (int i = id - 1; i < contador - 1; i++)
        {
            contactos[i] = contactos[i + 1];
        }
        contador--;
        Console.WriteLine("Contacto borrado con éxito.");
    }

    public static void listarcontacto()
    {
        Console.WriteLine("ID\tNombre\tTelefono\tEmail");
        Console.WriteLine("============================================");
        for (int i = 0; i < contador; i++)
        {
            Contacto contacto = contactos[i];
            Console.WriteLine($"{contacto.Id}\t{contacto.Nombre}\t{contacto.Telefono}\t{contacto.Email}");
        }
    }

    public static void buscarcontacto()
    {
        Console.WriteLine("Ingrese el término de búsqueda:");
        string busqueda = Console.ReadLine().ToLower();
        Console.WriteLine("ID\tNombre\tTelefono\tEmail");
        Console.WriteLine("============================================");
        for (int i = 0; i < contador; i++)
        {
            Contacto contacto = contactos[i];
            if (contacto.Nombre.ToLower().Contains(busqueda) || contacto.Telefono.ToLower().Contains(busqueda) || contacto.Email.ToLower().Contains(busqueda))
            {
                Console.WriteLine($"{contacto.Id}\t{contacto.Nombre}\t{contacto.Telefono}\t{contacto.Email}");
            }
        }
    }

    public static void guardarcontacto()
    {
        using (StreamWriter writer = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < contador; i++)
            {
                Contacto contacto = contactos[i];
                writer.WriteLine($"{contacto.Id},{contacto.Nombre},{contacto.Telefono},{contacto.Email}");
            }
        }
        Console.WriteLine("Contactos guardados exitosamente en agenda.csv.");
    }
}