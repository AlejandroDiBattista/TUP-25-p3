using System.Console;     
using System.IO;    

struct contacto
{
    public string nombre;
    public string apellido;
    public string telefono;
    public string email;
}
 class Program
    {

 const int MaxContactos = 100;
 static Contacto[] contactos = new Contacto[MaxContactos];   
  static int cantidadContactos = 0;
      
 static int siguienteId = 1;
 static string archivoAgenda = "agenda.csv";
   }
  static void AgregarContacto()
{
    if (cantidadContactos >= MaxContactos)
       {
        Console.WriteLine("Límite de contactos alcanzado.");
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();          
         return;
        }
            Contacto nuevoContacto = new Contacto();
            nuevoContacto.Id = siguienteId;
            siguienteId++;
            Console.Write("Ingrese el nombre: ");
            nuevoContacto.Nombre = Console.ReadLine();
            Console.Write("Ingrese el teléfono: ");
            nuevoContacto.Telefono = Console.ReadLine();
            Console.Write("Ingrese el email: ");
            nuevoContacto.Email = Console.ReadLine();
             contactos[cantidadContactos] = nuevoContacto;
            cantidadContactos++;
            Console.WriteLine("Contacto agregado correctamente. Presione Enter para continuar...");
            Console.ReadLine();
            }
            
            static void ModificarContacto()
        {
            Console.Write("Ingrese el ID del contacto a modificar: ");
            int idModificar;
            if (!int.TryParse(Console.ReadLine(), out idModificar))
            {
                Console.WriteLine("ID inválido. Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }
            int indice = -1;
            for (int i = 0; i < cantidadContactos; i++)
            {
                if (contactos[i].Id == idModificar)
                {
                    indice = i;
                    break;
                }
            }

            if (indice == -1)
            {
                Console.WriteLine("Contacto no encontrado. Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }
               Console.WriteLine("Deje en blanco para mantener el valor actual.");
            Console.Write("Nombre (" + contactos[indice].Nombre + "): ");
            string nuevoNombre = Console.ReadLine();
            if (nuevoNombre != "")
                contactos[indice].Nombre = nuevoNombre;

            Console.Write("Teléfono (" + contactos[indice].Telefono + "): ");
            string nuevoTelefono = Console.ReadLine();
            if (nuevoTelefono != "")
                contactos[indice].Telefono = nuevoTelefono;

            Console.Write("Email (" + contactos[indice].Email + "): ");
            string nuevoEmail = Console.ReadLine();
            if (nuevoEmail != "")
                contactos[indice].Email = nuevoEmail;

            Console.WriteLine("Contacto modificado correctamente. Presione Enter para continuar...");
            Console.ReadLine();
        }
         static void BorrarContacto()
        {
            Console.Write("Ingrese el ID del contacto a borrar: ");
            int idBorrar;
            if (!int.TryParse(Console.ReadLine(), out idBorrar))
            {
                Console.WriteLine("ID inválido. Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }  int indice = -1;
            for (int i = 0; i < cantidadContactos; i++)
            {
                if (contactos[i].Id == idBorrar)
                {
                    indice = i;
                    break;
                }
            }

            if (indice == -1)
            {
                Console.WriteLine("Contacto no encontrado. Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }for (int i = indice; i < cantidadContactos - 1; i++)
            {
                contactos[i] = contactos[i + 1];
            }
            cantidadContactos--;

            Console.WriteLine("Contacto borrado correctamente. Presione Enter para continuar...");
            Console.ReadLine();
            
        }
          static void ListarContactos()
        {
            Console.Clear();
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Teléfono", "Email");
            Console.WriteLine(new string('-', 70));
            for (int i = 0; i < cantidadContactos; i++)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", 
                    contactos[i].Id, 
                    contactos[i].Nombre, 
                    contactos[i].Telefono, 
                    contactos[i].Email);
            }
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
            }

static void BuscarContacto()
        {
            Console.Write("Ingrese término de búsqueda: ");
            string termino = Console.ReadLine().ToLower();

            Console.Clear();
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Teléfono", "Email");
            Console.WriteLine(new string('-', 70));
            for (int i = 0; i < cantidadContactos; i++)
            {if (contactos[i].Nombre.ToLower().IndexOf(termino) >= 0 ||
                    contactos[i].Telefono.ToLower().IndexOf(termino) >= 0 ||
                    contactos[i].Email.ToLower().IndexOf(termino) >= 0)
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}",
                        contactos[i].Id,
                        contactos[i].Nombre,
                        contactos[i].Telefono,
                        contactos[i].Email);
                }
            }
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }
  static void CargarContactos()
        {
            if (!File.Exists(archivoAgenda))
                return;
            try
            {
                string[] lineas = File.ReadAllLines(archivoAgenda);
                for (int i = 0; i < lineas.Length && cantidadContactos < MaxContactos; i++)
                {
                    string linea = lineas[i];
                    string[] partes = linea.Split(',');
                    if (partes.Length == 4)
                    {
                        Contacto c = new Contacto();
                        int id;
                        if (int.TryParse(partes[0], out id))
                        {
                            c.Id = id;
                            c.Nombre = partes[1];
                            c.Telefono = partes[2];
                            c.Email = partes[3];
                            contactos[cantidadContactos] = c;
                            cantidadContactos++;
                             if (id >= siguienteId)
                                siguienteId = id + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar contactos: " + ex.Message);
                Console.WriteLine("Presione Enter para continuar...");
                Console.ReadLine();
            }
        }
 static void GuardarContactos()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(archivoAgenda))
                {
                    for (int i = 0; i < cantidadContactos; i++)
                    { sw.WriteLine("{0},{1},{2},{3}",
                            contactos[i].Id,
                            contactos[i].Nombre,
                            contactos[i].Telefono,
                            contactos[i].Email);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar contactos: " + ex.Message);
                Console.WriteLine("Presione Enter para finalizar...");
                Console.ReadLine();
            }
        } static void Main(string[] args)
        {
            CargarContactos();
            bool salir = false;
            while(!salir)
             {
                Console.Clear();
                Console.WriteLine("Agenda de Contactos");
                Console.WriteLine("===================");
                Console.WriteLine("1. Agregar contacto");
                Console.WriteLine("2. Modificar contacto");
                Console.WriteLine("3. Borrar contacto");
                Console.WriteLine("4. Listar contactos");
                Console.WriteLine("5. Buscar contacto");
                Console.WriteLine("6. Salir");
                Console.Write("Elija una opción: ");
                string opcion = Console.ReadLine();

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
                  
                    GuardarContactos();
                    salir = true;
                }
                else
                {
                    Console.WriteLine("Opción no válida. Presione Enter para continuar...");
                    Console.ReadLine();
                }
            }
        }
    