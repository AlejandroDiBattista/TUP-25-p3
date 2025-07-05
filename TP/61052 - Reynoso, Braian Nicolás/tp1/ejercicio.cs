using System;
using System.IO;

public struct contacto
{
    public int id { get; set; }
    public string nombre { get; set; }
    public string telefono { get; set; }
    public string email { get; set; }

    public contacto(int id ,string nombre, string telefono, string email)
    {
        this.id = id;
        this.nombre = nombre;
        this.telefono = telefono;
        this.email = email;
    }
}

public class funcionesparaContactos ()
{
    public static contacto[] contactos = new contacto[100];
    static int cantidadContactos = 0;
    string archivo = "agenda.csv";

    public static void cargarcontactos()
    {
        if (!File.Exists("agenda.csv")) 
        {
            Console.WriteLine("No se encontró el archivo de contactos. Se creará uno nuevo al guardar.");
            return;
        }

        string[] lineas = File.ReadAllLines("agenda.csv");

        if (lineas.Length == 0) 
        {
            Console.WriteLine("El archivo está vacío. No se cargaron contactos.");
            return;
        }

        for (int i = 0; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split(';'); // Separar los datos por el delimitador ';'

            if (datos.Length == 4 && int.TryParse(datos[0], out int id)) // Validar que haya 4 campos y que el ID sea un número
            {
                contactos[cantidadContactos] = new contacto(
                    id,         // ID convertido a entero
                    datos[1],    // Nombre
                    datos[2],    // Teléfono
                    datos[3]     // Email
                );
                cantidadContactos++;
            }
            else
            {
                Console.WriteLine($"Línea inválida en el archivo: {lineas[i]}");
            }
        }

        Console.WriteLine("Contactos cargados desde el archivo agenda.csv.");
    }

    public static void agregarcontacto()
    {
        if (cantidadContactos < contactos.Length)
        {
            Console.WriteLine("Puedes Agregar un Contacto");
            contacto nuevocontacto = new contacto();

            nuevocontacto.id = cantidadContactos + 1; // Asignar un ID único al nuevo contacto
            Console.Write("Ingresar el nombre completo del contacto: ");
            nuevocontacto.nombre = Console.ReadLine();
            Console.Write("Ingresa el número de teléfono del contacto: ");
            nuevocontacto.telefono = Console.ReadLine();
            Console.Write("Ingresar el email del contacto: ");
            nuevocontacto.email = Console.ReadLine();
            nuevocontacto.id = cantidadContactos + 1; 
            contactos[cantidadContactos] = nuevocontacto;
            cantidadContactos++;
            funcionesparaContactos.guardarcontactos(); 

            Console.WriteLine($"El contacto con ID {nuevocontacto.id} se guardó con éxito.");
        }
        else
        {
            Console.WriteLine("Ya no se pueden agregar más contactos, la agenda está llena.");
        }
    }

    public static void ModificarContacto()
    {
        Console.WriteLine("Puedes Modificár un Contacto ");
        Console.Write("Ingresar el ID del contacto que desea modificár: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            bool idmodificar = false;
            for (int i = 0; i < cantidadContactos; i++)
            {
                if (contactos[i].id == id)
                {
                    idmodificar = true;
                    Console.WriteLine($"Contacto seleccionado: {contactos[i].nombre}, {contactos[i].telefono}, {contactos[i].email}");

                    Console.Write("Ingrese un nuevo nombre (o presione Enter si no desea modificar): ");
                    string nuevonombre = Console.ReadLine();
                    if (!string.IsNullOrEmpty(nuevonombre))
                    {
                        contactos[i].nombre = nuevonombre;
                    }

                    Console.Write("Ingrese un nuevo número de teléfono (o presione Enter si no desea modificar): ");
                    string nuevotelefono = Console.ReadLine();
                    if (!string.IsNullOrEmpty(nuevotelefono))
                    {
                        contactos[i].telefono = nuevotelefono;
                    }

                    Console.Write("Ingresar el nuevo email (o presione Enter si no desea modificar): ");
                    string nuevoemail = Console.ReadLine();
                    if (!string.IsNullOrEmpty(nuevoemail))
                    {
                        contactos[i].email = nuevoemail;
                    }
                          
                    Console.WriteLine("El Contacto ha sido modificado correctamente.");
                    funcionesparaContactos.guardarcontactos();
                    break;
                }

            }

            if (!idmodificar)
            {
                Console.WriteLine("No se encontró un contacto con el ID que se ingresó.");
            }
        }
        else
        {
            Console.WriteLine("Entrada no válida. Debe ingresar un número.");
        }
    }

    public static void borrarContacto ()
    {
         Console.WriteLine("Puedes Borrar un Contacto");
            Console.Write("Ingresar el ID del contacto que desea Borrar: ");

              int indice = -1;

              if(!int.TryParse(Console.ReadLine(), out int id))
             {
                Console.WriteLine("El id ingresado es incorrecto, prosiga a ingresar un id de un contacto existente.");
                return;
             }

              for (int i = 0; i < cantidadContactos; i++)
              {
                  if (contactos[i].id == id)
                  {
                      indice = i;
                      break;
                  }
              }
              if (indice == -1)
              {
                Console.WriteLine("No se encontró el contacto con el ID que se ingresó");
                return;
              }
              for (int i = indice; i < cantidadContactos - 1; i++)
               {
                 contactos[i] = contactos[i + 1];
                 contactos[i].id = i + 1; 
                }

        cantidadContactos--; 
        funcionesparaContactos.guardarcontactos();
        Console.WriteLine($"El contacto con ID {id} ha sido eliminado correctamente.");
    }


    public static void ListarContactos()
    {
         Console.WriteLine("Lista de Contactos");
         
     Console.WriteLine("ID     | Nombre           | Teléfono        | Email");
     Console.WriteLine("----------------------------------------------------");

     for (int i = 0; i < cantidadContactos; i++)
     {
        contacto contacto = contactos[i];
        Console.WriteLine($"{contacto.id,-6} | {contacto.nombre,-15} | {contacto.telefono,-15} | {contacto.email}");
     }
    }

     public static void buscarcontactos()
   {
       Console.WriteLine("Puedes buscar un contacto por nombre o teléfono.");
       Console.Write("Ingrese el ID del contacto que desea buscar: ");
         if (int.TryParse(Console.ReadLine(), out int id))
       {
              bool idbuscar = false;
              for (int i = 0; i < cantidadContactos; i++)
              {
                if (contactos[i].id == id)
                {
                     Console.WriteLine($"Contacto encontrado: ID: {contactos[i].id}, Nombre: {contactos[i].nombre}, Teléfono: {contactos[i].telefono}, Email: {contactos[i].email}");
                     idbuscar = true;
                     break;
                }
              }
    
              if (!idbuscar)
              {
                Console.WriteLine("No se encontró ningun contacto con el ID que se ingresó.");
              }
        }
         else
           {
              Console.WriteLine("Entrada no válida. Debe ingresar un número.");
            }
    }
    
    public static void guardarcontactos()
{
    try
    {
        using (StreamWriter writer = new StreamWriter("agenda.csv", false)) // 'false' sobrescribe el archivo correctamente
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                writer.WriteLine($"{contactos[i].id};{contactos[i].nombre};{contactos[i].telefono};{contactos[i].email}");
            }
        }
        Console.WriteLine("Contactos guardados en el archivo agenda.csv correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al guardar contactos: {ex.Message}");
    }
}


}

funcionesparaContactos.cargarcontactos();

bool salir = false;
        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("Bienvenido a la agenda de contactos");
            Console.WriteLine("1. Agregár contacto");
            Console.WriteLine("2. Modificár contacto");
            Console.WriteLine("3. Borrár contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("0. Salír");
            Console.Write("Seleccione una opción: ");

            if (int.TryParse(Console.ReadLine(), out int elegiropcion))
            {
                switch (elegiropcion)
                {
                    case 1:
                        funcionesparaContactos.agregarcontacto();
                        break;
                    case 2:
                        funcionesparaContactos.ModificarContacto();
                        break;
                    case 3:
                        funcionesparaContactos.borrarContacto();
                        break;
                    case 4:
                        funcionesparaContactos.ListarContactos();
                        break;
                    case 5:
                        funcionesparaContactos.buscarcontactos();
                        break;
                    case 0:
                        Console.WriteLine("Saliendo de la agenda...");
                        funcionesparaContactos.guardarcontactos();
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Esta opcion no es valida, ingrese un número entre 1 y 3.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Entrada no válida, ingrese un número porfavor.");
            }

            if (!salir)
            {
                Console.WriteLine("Presione una tecla para continuar...");
                Console.ReadKey();
            }


        }