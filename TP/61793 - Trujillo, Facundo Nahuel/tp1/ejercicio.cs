using static System.Console;

struct Contacto{
    public int Id;
    public string Nombre;
    public string Email;
    public string Telefono;
}
class AgendaTp1
{

    const int MContactos = 100;
    static Contacto[] Agenda = new Contacto[MContactos];
    static int NumContactos = 0;
    static int SigID = 1;

    static void Main()
    {
        Clear();
        CargarDesdeArchivo();
        int opcion;
        do
        {
            Console.WriteLine("==== AGENDA DE CONTACTOS ====");
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contactos");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("6) Salir");
            Console.Write("Seleccione una opcion: ");
            
            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                switch (opcion)
                {
                    case 1: AgregarContacto(); break;
                    case 2: ModificarContacto(); break;
                    case 3: BorrarContacto(); break;
                    case 4: ListarContacto(); break;
                    case 5: BuscarContacto(); break;
                    case 6: GuardarArchivo(); break;
                    default: Console.WriteLine("Opcion no valida"); break;
                }
            }
            else
            {
            Console.WriteLine("Entrada no valida.");
            }
        } while (opcion != 6);
    } 
     static void AgregarContacto()
{
    if (NumContactos >= MContactos) 
    {
        Console.WriteLine("La agenda se lleno");
        return;
    }

    Contacto nuevo;
    nuevo.Id = SigID++;
    Console.Write("Nombre: "); nuevo.Nombre = Console.ReadLine();
    Console.Write("Telefono: "); nuevo.Telefono = Console.ReadLine();
    Console.Write("Email: "); nuevo.Email = Console.ReadLine();

    Agenda[NumContactos++] = nuevo; 
    Console.WriteLine("Contacto agregado");
}
    static void ModificarContacto()
    {
        Console.Write("Ingrese ID del contacto para modificar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            for (int i = 0; i < NumContactos; i++)
            {
                if (Agenda[i].Id == id)
                {
                    Console.Write("Nuevo nombre: ");
                    string nuevoNombre = Console.ReadLine();
                    if (!string.IsNullOrEmpty(nuevoNombre)) Agenda[i].Nombre = nuevoNombre;
                    Console.Write("Nuevo telefono: ");
                    string nuevoTelefono = Console.ReadLine();
                    if (!string.IsNullOrEmpty(nuevoTelefono)) Agenda[i].Telefono = nuevoTelefono;
                    Console.Write("Nuevo email: ");
                    string nuevoEmail = Console.ReadLine();
                    if (!string.IsNullOrEmpty(nuevoEmail)) Agenda[i].Email = nuevoEmail;
                    Console.WriteLine("Contacto modificado");
                    return;
                }
            }
            Console.WriteLine("No se encontro el ID");
        }
        else
        {
            Console.WriteLine("Datos no validos");
        }
    }
    static void BorrarContacto()
    {
        Console.Write("Ingrese ID del contacto a borrar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            for (int i = 0; i < NumContactos; i++)
            {
                if (Agenda[i].Id == id)
                {
                    for (int j = i; j < NumContactos - 1; j++)
                    {
                        Agenda[j] = Agenda[j + 1];
                    }
                    NumContactos--;
                    Console.WriteLine("Contacto eliminado");
                    return;
                }
            }
            Console.WriteLine("No se encontro el ID");
        }
        else
        {
            Console.WriteLine("Datos no validos");
        }
    }
     static void ListarContacto()
    {
        Console.WriteLine("\nID       Nombre           Telefono        Email");
        Console.WriteLine("-------------------------------------------------");
        for (int i = 0; i < NumContactos; i++)
        {
            Console.WriteLine($"{Agenda[i].Id}    {Agenda[i].Nombre}    {Agenda[i].Telefono}    {Agenda[i].Email}");
            Console.WriteLine("-------------------------------------------------");
        }
    }
   static void BuscarContacto()
{
    Console.Write("Ingrese algun dato para busqueda: ");
    string termino = Console.ReadLine().ToLower();
    Console.WriteLine("\nID        Nombre           Teléfono        Email");
    Console.WriteLine("--------------------------------------------------");

    for (int i = 0; i < NumContactos; i++)
    {
        if (Agenda[i].Nombre.ToLower().Contains(termino) || 
            Agenda[i].Telefono.Contains(termino) || 
            Agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{Agenda[i].Id} {Agenda[i].Nombre} {Agenda[i].Telefono} {Agenda[i].Email}");
        }
    }
}
    static void CargarDesdeArchivo()
{
    if (File.Exists("agenda.csv"))
    {
        string[] lineas = File.ReadAllLines("agenda.csv");
        
        for (int i = 1; i < lineas.Length; i++) 
        {
            string[] datos = lineas[i].Split(',');
            if (datos.Length == 4)
            {
                Contacto c;
                c.Id = int.Parse(datos[0]);
                c.Nombre = datos[1];
                c.Telefono = datos[2];
                c.Email = datos[3];
                Agenda[NumContactos++] = c;
                SigID = c.Id + 1;
            }
        }
    }
}
    static void GuardarArchivo()
{
    using (StreamWriter sw = new StreamWriter("agenda.csv"))
    {
        sw.WriteLine("ID, Nombre, Telefono, Email"); 
        for (int i = 0; i < NumContactos; i++)
        {
            sw.WriteLine($"{Agenda[i].Id},{Agenda[i].Nombre},{Agenda[i].Telefono},{Agenda[i].Email}");
        }
    }
    Console.WriteLine("Se guardo la agrenda");
}

}
