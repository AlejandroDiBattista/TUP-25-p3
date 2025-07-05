
using static System.Console;
using System.IO;

public struct Usuario
{
    private static int SumaId = 1;
    public int Id { get; private set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }

    public Usuario(string nombre, string apellido, string telefono, string email)
    {
        Id = SumaId++;
        Nombre = nombre;
        Apellido = apellido;
        Telefono = telefono;
        Email = email;
    }
}

Usuario[] usuarios = new Usuario[10];
int contador = 0;
string archivo = "agenda.csv";

void CargarUsuariosDesdeArchivo()
{
    if (!File.Exists(archivo)) return;

    string[] lineas = File.ReadAllLines(archivo);

    if (lineas.Length <= 1) return;

    contador = 0;

    for (int i = 1; i < lineas.Length; i++)
    {
        string[] datos = lineas[i].Split(',');

        if (datos.Length == 5)
        {
            usuarios[contador] = new Usuario(datos[1], datos[2], datos[3], datos[4]);
            contador++;
        }
    }
}


void GuardarUsuariosEnArchivo()
{
    using (StreamWriter writer = new StreamWriter(archivo, append: false)) 
    {        
        if (new FileInfo(archivo).Length == 0)
        {
            writer.WriteLine("------------------------------------------------------------");
            writer.WriteLine("| ID  | Nombre             | Apellido          | Teléfono       | Email               |");
            writer.WriteLine("------------------------------------------------------------");
        }

        // Escribir los datos de los usuarios
        for (int i = 0; i < contador; i++)
        {
            Usuario usuario = usuarios[i];
            writer.WriteLine($"{usuario.Id}, {usuario.Nombre}, {usuario.Apellido}, {usuario.Telefono}, {usuario.Email}");
        }

        writer.WriteLine("------------------------------------------------------------");
    }

    WriteLine("✅ Contactos guardados en agenda.csv.");
}

WriteLine("📂 Datos cargados desde agenda.csv");

CargarUsuariosDesdeArchivo();

Clear();
while (true)
{
    Clear();
    WriteLine("----AGENDA DE USUAIO----");
    WriteLine("1. Ingresar usuario");
    WriteLine("--------------------");
    WriteLine("2. Mostrar usuarios");
    WriteLine("--------------------");
    WriteLine("3. Modificar usuario");
    WriteLine("--------------------");
    WriteLine("4. Eliminar usuario");
    WriteLine("--------------------");
    WriteLine("5. Buscar usuario");
    WriteLine("--------------------");
    WriteLine("6. Salir");
    WriteLine("--------------------");
    Write("Opción: ");
    string opcion = ReadLine();

    if (opcion == "1")
    {
        Clear();
        Write("Nombre: ");
        string nombre = ReadLine();
        Write("Apellido: ");
        string apellido = ReadLine();
        Write("Teléfono: ");
        string telefono = ReadLine();
        Write("Email: ");
        string email = ReadLine();
        usuarios[contador] = new Usuario(nombre, apellido, telefono, email);
        contador++;
        GuardarUsuariosEnArchivo();
    }
    if (opcion == "2")
    {
        Clear();
        WriteLine("Usuarios registrados:");
        for (int j = 0; j < contador; j++)
        {
            WriteLine($"ID: {usuarios[j].Id}, Nombre: {usuarios[j].Nombre}, Apellido: {usuarios[j].Apellido}, Teléfono: {usuarios[j].Telefono}, Email: {usuarios[j].Email}");
        }
        WriteLine("Presione una tecla para continuar.");
        ReadKey();
    }
    if (opcion == "3")
    {
        Clear();
        Write("Ingrese el ID del usuario a modificar: ");
        int id = int.Parse(ReadLine());
        bool usuarioEncontrado = false;
        for (int j = 0; j < contador; j++)
        {
            if (usuarios[j].Id == id)
            {
                usuarioEncontrado = true;
                Clear();
                WriteLine($"Usuario encontrado: ID: {usuarios[j].Id}, Nombre: {usuarios[j].Nombre}, Apellido: {usuarios[j].Apellido}, Teléfono: {usuarios[j].Telefono}, Email: {usuarios[j].Email}");
                WriteLine("Seleccione el campo que desea modificar:");
                WriteLine("1. Nombre");
                WriteLine("2. Apellido");
                WriteLine("3. Teléfono");
                WriteLine("4. Email");
                Write("Opción: ");
                string opcionModificar = ReadLine();
                switch (opcionModificar)
                {
                    case "1":
                        Write("Nuevo Nombre: ");
                        usuarios[j].Nombre = ReadLine();
                        break;
                    case "2":
                        Write("Nuevo Apellido: ");
                        usuarios[j].Apellido = ReadLine();
                        break;
                    case "3":
                        Write("Nuevo Teléfono: ");
                        usuarios[j].Telefono = ReadLine();
                        break;
                    case "4":
                        Write("Nuevo Email: ");
                        usuarios[j].Email = ReadLine();
                        break;
                    default:
                        WriteLine("Opción no válida.");
                        break;
                }
                GuardarUsuariosEnArchivo();
                WriteLine("Usuario modificado correctamente.");
                WriteLine("Presione una tecla para continuar.");
                ReadKey();
                break;
            }
        }
        if (!usuarioEncontrado)
        {
            WriteLine("Usuario no encontrado.");
            WriteLine("Presione una tecla para continuar.");
            ReadKey();
        }
    }
    if (opcion == "4")
    {
        Clear();
        Write("Ingrese el ID del usuario a eliminar: ");
        int id = int.Parse(ReadLine());
        bool usuarioEncontrado = false;
        for (int j = 0; j < contador; j++)
        {
            if (usuarios[j].Id == id)
            {
                usuarioEncontrado = true;

                for (int k = j; k < contador - 1; k++)
                {
                    usuarios[k] = usuarios[k + 1];
                }
                contador--;
                GuardarUsuariosEnArchivo();
                WriteLine("Usuario eliminado correctamente.");
                break;
            }
        }
        if (!usuarioEncontrado)
        {
            WriteLine("Usuario no encontrado.");
        }

        WriteLine("Presione una tecla para continuar.");
        ReadKey();
    }
    if (opcion == "5")
    {
        Clear();
        Write("Ingrese el ID del usuario a buscar: ");
        int id = int.Parse(ReadLine());
        for (int j = 0; j < contador; j++)
        {
            if (usuarios[j].Id == id)
            {
                WriteLine($"ID: {usuarios[j].Id}, Nombre: {usuarios[j].Nombre}, Apellido: {usuarios[j].Apellido}, Teléfono: {usuarios[j].Telefono}, Email: {usuarios[j].Email}");
            }
        }
        WriteLine("Presione una tecla para continuar.");
        ReadKey();
    }
    if (opcion == "6")
    {
        Clear();
        GuardarUsuariosEnArchivo();
        break;
    }
}

WriteLine("El programa ha finalizado.");
ReadKey();