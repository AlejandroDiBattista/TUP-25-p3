using System; // Para usar la consola  (Console)
using System.IO; // Para leer archivos    (File)

Console.Clear();

// Ayuda:
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

struct Contacto
{
  public int Id { get; private set; }
  public string Nombre { get; set; }
  public ulong Telefono { get; set; }
  public string Email { get; set; }

  static int proximoId = 1;

  public Contacto()
  {
    Id = proximoId++;
  }

  public Contacto(string nombre, ulong telefono)
  {
    Id = proximoId++;
    Nombre = nombre;
    Telefono = telefono;
  }

  public Contacto(string nombre, string email)
  {
    Id = proximoId++;
    Nombre = nombre;
    Email = email;
  }

  public Contacto(string nombre, ulong telefono, string email)
  {
    Id = proximoId++;
    Nombre = nombre;
    Telefono = telefono;
    Email = email;
  }

  public override string ToString()
  {
    string tel = Telefono != 0 ? Telefono.ToString() : "";
    return $"Nombre: {Nombre}, Teléfono: {tel}, Email: {Email}";
  }

  public void MostrarTabulado()
  {
    Console.WriteLine("{0, -3}|{1, -20}|{2, -15}|{3, -30}", Id, Nombre, Telefono, Email);
  }
}

struct Agenda
{
  public Contacto[] Contactos { get; private set; }
  const uint MAX_CONTACTOS = 10;
  int _cantidad = 1;

  const string FICHERO = "./agenda.csv";

  public int Cantidad
  {
    get => _cantidad;
    private set => _cantidad = value;
  }

  public Agenda()
  {
    Console.WriteLine("===== AGENDA DE CONTACTOS =====");
    Contactos = new Contacto[MAX_CONTACTOS];
    Cantidad = 0;
    CargarContactosDesdeArchivo();
  }

  private void CargarContactosDesdeArchivo()
  {
    if (File.Exists(FICHERO))
    {
      string[] contenido = File.ReadAllLines(FICHERO);
      int contactosCargados = 0;

      for (int i = 1; i < contenido.Length; i++)
      {
        if (!string.IsNullOrEmpty(contenido[i]))
        {
          string[] linea = contenido[i].Split(",");

          AgregarContacto(linea[0], linea[1], linea[2]);
          contactosCargados++;
        }
      }

      Console.WriteLine($"Cargados {contactosCargados} contactos ya existentes.");
    }
    else
    {
      Console.WriteLine($"[WARNING]: El fichero \"{FICHERO}\" con los contactos no existe. No se cargarán contactos.");
    }
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
  }

  private void GuardarContactosEnArchivo()
  {
    Console.Clear();
    Console.WriteLine("===== AGENDA DE CONTACTOS =====");
    if (File.Exists(FICHERO))
    {
      string[] contenido = new string[Cantidad + 1];
      int contactosGuardados = 0;

      contenido[0] = "nombre,telefono,email";

      for (int i = 0; i < Cantidad; i++)
      {
        string linea = Contactos[i].Nombre + "," + Contactos[i].Telefono.ToString() + "," + Contactos[i].Email;
        contenido[++contactosGuardados] = linea;
      }

      File.WriteAllLines(FICHERO, contenido);

      Console.WriteLine($"Guardados {contactosGuardados} contactos en el fichero.");
    }
    else
    {
      Console.WriteLine($"[WARNING]: El fichero \"{FICHERO}\" no existe. No se guardarán contactos.");
    }
  }

  public void Salir()
  {
    GuardarContactosEnArchivo();
    Console.WriteLine("Saliendo. Hasta pronto...");
  }

  public void MostrarAgenda()
  {
    Console.Clear();
    Console.WriteLine("=== Lista de Contactos ===");
    Console.WriteLine("{0, -3}|{1, -20}|{2, -15}|{3, -30}", "ID", "Nombre", "Teléfono", "Email");
    for (int i = 0; i < Cantidad; i++)
    {
      Contactos[i].MostrarTabulado();
    }
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
  }

  public void AgregarContacto()
  {
    Console.Clear();
    Console.WriteLine("=== Agregar Contacto ===");
    Contacto nuevo = new Contacto();
    Console.Write("Nombre: ");
    nuevo.Nombre = Console.ReadLine();
    Console.Write("Teléfono: ");
    nuevo.Telefono = (ulong)long.Parse(Console.ReadLine());
    Console.Write("Email: ");
    nuevo.Email = Console.ReadLine();
    Contactos[Cantidad++] = nuevo;

    Console.WriteLine($"Contacto agregado con el ID: {nuevo.Id}");
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
  }

  public void AgregarContacto(string nombre, string telefono, string email)
  {
    Contacto nuevo = new Contacto();
    nuevo.Nombre = nombre;
    nuevo.Telefono = (ulong)long.Parse(telefono);
    nuevo.Email = email;
    Contactos[Cantidad++] = nuevo;
  }

  public void ModificarContacto()
  {
    Console.Clear();
    Console.WriteLine("=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    string busqueda = Console.ReadLine();
    if (busqueda == "" || busqueda == "\n")
    {
      Console.WriteLine("[Error]: No ingresó un valor de ID para modificar");
      Console.WriteLine("Presione cualquier tecla para continuar...");
      Console.ReadKey();
    }
    int id = int.Parse(busqueda);

    for (int i = 0; i < Cantidad; i++)
    {
      if (Contactos[i].Id == id)
      {
        Console.WriteLine($"Datos actuales => {Contactos[i]}");
        Console.WriteLine("(Deje el campo en blanco para no modificar)");

        Console.Write("Nombre: ");
        string entrada = Console.ReadLine();

        if (!(string.IsNullOrEmpty(entrada)) && entrada != "\n")
        {
          Contactos[i].Nombre = entrada;
        }
        Console.Write("Teléfono: ");
        entrada = Console.ReadLine();
        if (!string.IsNullOrEmpty(entrada) && entrada != "\n")
        {
          Contactos[i].Telefono = (ulong)long.Parse(entrada);
        }
        Console.Write("Email: ");
        entrada = Console.ReadLine();
        if (!string.IsNullOrEmpty(entrada) && entrada != "\n")
        {
          Contactos[i].Email = entrada;
        }

        Console.WriteLine($"Contacto modificado con éxito.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
        return;
      }
    }
    Console.WriteLine("[Error]: No ingresó un valor de ID existente para modificar");
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
  }

  public void BuscarContacto()
  {
    Console.Clear();
    Console.WriteLine("=== Buscar Contacto ===");
    Console.Write("Ingrese un termino de búsqueda (nombre, teléfono, email): ");
    string busqueda = Console.ReadLine();

    if (!string.IsNullOrEmpty(busqueda) && busqueda != "\n")
    {
      Console.WriteLine("=== Lista de Contactos ===");
      Console.WriteLine("{0, -3}|{1, -20}|{2, -15}|{3, -30}", "ID", "Nombre", "Teléfono", "Email");
      for (int i = 0; i < Cantidad; i++)
      {
        if (
          Contactos[i].Nombre.Contains(busqueda) ||
          Contactos[i].Telefono.ToString().Contains(busqueda) ||
          Contactos[i].Email.Contains(busqueda)
        )
        {
          Contactos[i].MostrarTabulado();
        }
      }
      Console.WriteLine("Presione cualquier tecla para continuar...");
      Console.ReadKey();
    }
    else
    {
      Console.WriteLine("[Error]: No ingresó un valor para buscar");
      Console.WriteLine("Presione cualquier tecla para continuar...");
      Console.ReadKey();
    }
  }

  public void EliminarContacto()
  {
    Console.Clear();
    Console.WriteLine("=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a borrar: ");
    string busqueda = Console.ReadLine();
    if (string.IsNullOrEmpty(busqueda) || busqueda == "\n")
    {
      Console.WriteLine("[Error]: No ingresó un valor de ID para eliminar");
      Console.WriteLine("Presione cualquier tecla para continuar...");
      Console.ReadKey();
    }
    int id = int.Parse(busqueda);
    for (int i = 0; i < Cantidad; i++)
    {
      if (Contactos[i].Id == id)
      {
        for (int j = i; j < Cantidad - 1; j++)
        {
          Contactos[j] = Contactos[j + 1];
        }
        Cantidad--;
        Console.WriteLine($"Contacto con el ID: {id} eliminado con éxito.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
        return;
      }
    }
    Console.WriteLine("[Error]: No se eliminó el contacto. Ingresó un ID inexistente o erroneo?");
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
  }
}

var agenda = new Agenda();
bool seguirEjecutando = true;

while (seguirEjecutando)
{
  Console.Clear();
  Console.WriteLine("===== AGENDA DE CONTACTOS =====");
  Console.WriteLine("1) Agregar contacto");
  Console.WriteLine("2) Modificar contacto");
  Console.WriteLine("3) Borrar contacto");
  Console.WriteLine("4) Listar contactos");
  Console.WriteLine("5) Buscar contacto");
  Console.WriteLine("0) Salir");
  Console.Write("Seleccione una opción: ");
  ConsoleKeyInfo opcion = Console.ReadKey();

  switch (opcion.Key)
  {
    case ConsoleKey.D1:
      {
        agenda.AgregarContacto();
        break;
      }
    case ConsoleKey.D2:
      {
        agenda.ModificarContacto();
        break;
      }
    case ConsoleKey.D3:
      {
        agenda.EliminarContacto();
        break;
      }
    case ConsoleKey.D4:
      {
        agenda.MostrarAgenda();
        break;
      }
    case ConsoleKey.D5:
      {
        agenda.BuscarContacto();
        break;
      }
    case ConsoleKey.D0:
      {
        seguirEjecutando = false;
        agenda.Salir();
        break;
      }
    default:
      {
        Console.WriteLine("\n[Error]: Ingresó una opción equivocada.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        break;
      }
  }
}
