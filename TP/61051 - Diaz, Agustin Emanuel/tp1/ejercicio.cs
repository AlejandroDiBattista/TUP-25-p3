using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)
using System.Text.RegularExpressions;
using System.Threading;
// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)



// =============================================
// ESTRUCTURAS Y CONSTANTES
// =============================================
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MAX_CONTACTOS = 100;
Contacto[] agenda = new Contacto[MAX_CONTACTOS];
int cantidadContactos = 0;
int ultimoId = 0;

// =============================================
// FUNCIONES DE VALIDACIÓN
// =============================================
bool EsNombreValido(string nombre)
{
    return Regex.IsMatch(nombre, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$");
}

bool EsTelefonoValido(string telefono)
{
    return Regex.IsMatch(telefono, @"^[\d-]+$") && telefono.Any(char.IsDigit);
}

bool EsEmailValido(string email)
{
    return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$");
}

string CapitalizarNombre(string nombre)
{
    return string.Join(" ", nombre.Split(' ')
        .Select(word => word.Length > 0 
            ? char.ToUpper(word[0]) + word.Substring(1).ToLower() 
            : ""));
}

// =============================================
// FUNCIONES DE MANEJO DE ARCHIVOS
// =============================================
void CargarContactosDesdeArchivo()
{
    if (!File.Exists("agenda.csv")) return;

    try
    {
        // Solo reiniciamos el array y contador, NO ultimoId
        agenda = new Contacto[MAX_CONTACTOS];
        cantidadContactos = 0;
        // ultimoId NO se reinicia aquí

        string[] lineas = File.ReadAllLines("agenda.csv");

        for (int i = 0; i < lineas.Length && cantidadContactos < MAX_CONTACTOS; i++)
        {
            string linea = lineas[i].Trim();
            
            if (string.IsNullOrEmpty(linea)) continue;

            // Procesar last_id PRIMERO
            if (linea.StartsWith("#last_id="))
            {
                string[] partes = linea.Split('=');
                if (partes.Length == 2 && int.TryParse(partes[1], out int id))
                {
                    ultimoId = id; // Actualizamos ultimoId aquí
                }
                continue;
            }

            // Resto del código para procesar contactos...
            string[] campos = linea.Split(',');
            if (campos.Length >= 4)
            {
                // Simplificamos la lectura usando Split
                string idStr = campos[0].Trim('"', ' ');
                string nombre = campos[1].Trim('"', ' ');
                string telefono = campos[2].Trim('"', ' ');
                string email = campos[3].Trim('"', ' ');

                if (int.TryParse(idStr, out int idContacto) &&
                    EsNombreValido(nombre) && 
                    EsTelefonoValido(telefono) && 
                    EsEmailValido(email))
                {
                    agenda[cantidadContactos] = new Contacto
                    {
                        Id = idContacto,
                        Nombre = nombre,
                        Telefono = telefono,
                        Email = email
                    };
                    
                    cantidadContactos++;
                    
                    // Actualizamos ultimoId si es necesario
                    if (idContacto > ultimoId)
                    {
                        ultimoId = idContacto;
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        MostrarError($"Error al cargar contactos: {ex.Message}");
    }
}

void GuardarContactosEnArchivo()
{
    try
    {
        // Preparamos el array de líneas a escribir
        string[] lineas = new string[cantidadContactos + 1]; // +1 para la línea de last_id
        
        // Primera línea: metadata
        lineas[0] = $"#last_id={ultimoId}";
        
        // Líneas de contactos
        for (int i = 0; i < cantidadContactos; i++)
        {
            string nombre = agenda[i].Nombre;
            
            // Manejo de comillas si el nombre contiene comas
            if (nombre.Contains(",") || nombre.Contains("\""))
            {
                nombre = $"\"{nombre.Replace("\"", "\"\"")}\"";
            }
            
            lineas[i + 1] = $"{agenda[i].Id},{nombre},{agenda[i].Telefono},{agenda[i].Email}";
        }
        
        // Caso especial: agenda vacía
        if (cantidadContactos == 0)
        {
            lineas = new string[] { "#last_id=0" };
        }
        
        // Escribimos todas las líneas de una vez
        File.WriteAllLines("agenda.csv", lineas);
        
        MostrarExito($"{cantidadContactos} contactos guardados correctamente");
    }
    catch (Exception ex)
    {
        MostrarError($"Error al guardar: {ex.Message}");
    }
}

// =============================================
// FUNCIONES DE INTERFAZ DE USUARIO
// =============================================
void DibujarEncabezado(string titulo)
{
    int ancho = titulo.Length + 8;
    Console.WriteLine(new string('=', ancho));
    Console.WriteLine($"|   {titulo}   |");
    Console.WriteLine(new string('=', ancho));
}

void MostrarMenuPrincipal()
{
    Console.Clear();
    Console.WriteLine("============================================");
    Console.WriteLine("|           AGENDA DE CONTACTOS           |");
    Console.WriteLine("============================================");
    Console.WriteLine("|                                          |");
    Console.WriteLine("|  1. Agregar nuevo contacto               |");
    Console.WriteLine("|  2. Modificar contacto existente         |");
    Console.WriteLine("|  3. Eliminar contacto                    |");
    Console.WriteLine("|  4. Mostrar todos los contactos          |");
    Console.WriteLine("|  5. Buscar contacto                      |");
    Console.WriteLine("|  6. Salir del programa                   |");
    Console.WriteLine("|                                          |");
    Console.WriteLine("============================================");
}

void MostrarError(string mensaje)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n  [!] {mensaje}");
    Console.ResetColor();
}

void MostrarExito(string mensaje)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n  [✓] {mensaje}");
    Console.ResetColor();
}

// =============================================
// FUNCIONES DE OPERACIONES CON CONTACTOS
// =============================================
void AgregarContacto()
{
    Console.Clear();
    DibujarEncabezado("AGREGAR NUEVO CONTACTO");
    
    // Verificar inicialización del array
    if (agenda == null) agenda = new Contacto[MAX_CONTACTOS];
    
    if (cantidadContactos >= MAX_CONTACTOS)
    {
        MostrarError("Agenda llena");
        return;
    }

    string nombre;
    string telefono;
    string email;

    // Validación mejorada de nombre
    do
    {
        Console.Write("\n  Nombre completo: ");
        nombre = Console.ReadLine()?.Trim() ?? "";
        
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarError("El nombre no puede estar vacío");
            continue;
        }
        
        if (!EsNombreValido(nombre))
        {
            MostrarError("Solo se permiten letras, espacios y algunos caracteres especiales");
        }
    } while (string.IsNullOrWhiteSpace(nombre) || !EsNombreValido(nombre));

    // Validación de teléfono
    do
    {
        Console.Write("  Teléfono (ej. 123-4567890): ");
        telefono = Console.ReadLine()?.Trim() ?? "";
        
        if (!EsTelefonoValido(telefono))
        {
            MostrarError("Formato inválido. Use números y guiones (ej. 123-4567890)");
        }
    } while (!EsTelefonoValido(telefono));

    // Validación de email
    do
    {
        Console.Write("  Email (ej. nombre@dominio.com): ");
        email = Console.ReadLine()?.Trim() ?? "";
        
        if (!EsEmailValido(email))
        {
            MostrarError("Formato de email inválido. Ejemplo válido: nombre@dominio.com");
        }
    } while (!EsEmailValido(email));

    // Asignación del nuevo contacto
    ultimoId++;
    agenda[cantidadContactos] = new Contacto 
    { 
        Id = ultimoId,
        Nombre = CapitalizarNombre(nombre),
        Telefono = telefono,
        Email = email.ToLower()
    };
    cantidadContactos++;
    
    // Guardado automático
    GuardarContactosEnArchivo();
    
    MostrarExito($"Contacto agregado (ID: {ultimoId})");
    
    // Debug: mostrar estado actual
    Console.WriteLine($"\nDEBUG: Contactos en memoria: {cantidadContactos}, Último ID: {ultimoId}");
}

void ModificarContacto()
{
    Console.Clear();
    DibujarEncabezado("MODIFICAR CONTACTO");

    Console.Write("\n  Ingrese el ID del contacto a modificar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        MostrarError("ID inválido. Debe ser un número.");
        return;
    }

    int index = -1;
    for (int i = 0; i < cantidadContactos ; i++)
    {
        if (agenda[i].Id == id)
        {
            index = i;
            break;
        }
    }

    if (index == -1)
    {
        MostrarError("No se encontró ningún contacto con ese ID.");
        return;
    }

    Console.WriteLine("\n  Contacto seleccionado:");
    Console.WriteLine("  +----+-------------------+----------------+---------------------------+");
    Console.WriteLine("  | ID |      Nombre      |    Teléfono    |          Email           |");
    Console.WriteLine("  +----+-------------------+----------------+---------------------------+");
    Console.WriteLine($"  | {agenda[index].Id,2} | {agenda[index].Nombre,-17} | {agenda[index].Telefono,-14} | {agenda[index].Email,-25} |");
    Console.WriteLine("  +----+-------------------+----------------+---------------------------+");

    Console.Write("\n  Nuevo nombre (ENTER para mantener actual): ");
    string nuevoNombre = Console.ReadLine()?.Trim() ?? "";
    if (!string.IsNullOrEmpty(nuevoNombre))
    {
        if (EsNombreValido(nuevoNombre))
        {
            agenda[index].Nombre = CapitalizarNombre(nuevoNombre);
            MostrarExito("Nombre actualizado correctamente");
        }
        else
        {
            MostrarError("Nombre no modificado: formato inválido");
        }
    }

    string nuevoTelefono;
    bool telefonoValido;
    do
    {
        Console.Write("  Nuevo teléfono (ENTER para mantener actual): ");
        nuevoTelefono = Console.ReadLine()?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(nuevoTelefono))
        {
            break; 
        }
        
        telefonoValido = EsTelefonoValido(nuevoTelefono);
        if (telefonoValido)
        {
            agenda[index].Telefono = nuevoTelefono;
            MostrarExito("Teléfono actualizado correctamente");
        }
        else
        {
            MostrarError("Formato inválido. Use números y guiones (ej. 123-4567890)");
        }
    } while (!telefonoValido);

    string nuevoEmail;
    bool emailValido;
    do
    {
        Console.Write("  Nuevo email (ENTER para mantener actual): ");
        nuevoEmail = Console.ReadLine()?.Trim() ?? "";
        
        if (string.IsNullOrEmpty(nuevoEmail))
        {
            break; 
        }
        
        emailValido = EsEmailValido(nuevoEmail);
        if (emailValido)
        {
            agenda[index].Email = nuevoEmail.ToLower();
            MostrarExito("Email actualizado correctamente");
        }
        else
        {
            MostrarError("Formato de email inválido. Debe contener '@' y dominio válido");
        }
    } while (!emailValido);

    MostrarExito("Modificación de contacto completada");
}

void BorrarContacto()
{
    if (cantidadContactos == 0)
    {
        MostrarError("No hay contactos para borrar.");
        return;
    }
    Console.Write("Ingrese el ID del contacto a borrar: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        for (int i = 0; i < cantidadContactos ; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < cantidadContactos  - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                cantidadContactos --;
                Console.WriteLine("Contacto eliminado correctamente.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
}

void ListarContactos()
{
    Console.Clear();
    DibujarEncabezado("LISTADO DE CONTACTOS");
    
    if (cantidadContactos  == 0)
    {
        Console.WriteLine("\n  No hay contactos registrados en la agenda.");
        return;
    }

    Console.WriteLine("\n  +----+-------------------+----------------+---------------------------+");
    Console.WriteLine("  | ID |      Nombre      |    Teléfono    |          Email           |");
    Console.WriteLine("  +----+-------------------+----------------+---------------------------+");
    
    for (int i = 0; i < cantidadContactos ; i++)
    {
        Console.WriteLine($"  | {agenda[i].Id,2} | {agenda[i].Nombre,-17} | {agenda[i].Telefono,-14} | {agenda[i].Email,-25} |");
    }
    
    Console.WriteLine("  +----+-------------------+----------------+---------------------------+");
    Console.WriteLine($"\n  Total de contactos: {cantidadContactos }");
}

void BuscarContacto()
{
    Console.Clear();
    DibujarEncabezado("BUSCAR CONTACTO");

    Console.Write("\nIngrese término: ");
    string termino = (Console.ReadLine() ?? "").Trim().ToLower();

    if (string.IsNullOrEmpty(termino))
    {
        MostrarError("Término inválido");
        return;
    }

    bool encontrado = false;
    Console.WriteLine("\nID | Nombre | Teléfono | Email");
    Console.WriteLine("------------------------------");

    for (int i = 0; i < cantidadContactos; i++)
    {
        if (agenda[i].Id.ToString().Contains(termino) ||
            agenda[i].Nombre.ToLower().Contains(termino) ||
            agenda[i].Telefono.Contains(termino) ||
            agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{agenda[i].Id} | {agenda[i].Nombre} | {agenda[i].Telefono} | {agenda[i].Email}");
            encontrado = true;
        }
    }

    if (!encontrado) Console.WriteLine("No se encontraron resultados");
}

// =============================================
// PROGRAMA PRINCIPAL
// =============================================
while (true)
{  
    CargarContactosDesdeArchivo();
    MostrarMenuPrincipal();
    Console.Write("\n  Seleccione una opción (1-6): ");
    if (int.TryParse(Console.ReadLine(), out int opcion))
    {
        switch (opcion)
        {
            case 1: 
                AgregarContacto();
                break;
            case 2: 
                ModificarContacto();
                break;
            case 3: 
                BorrarContacto();
                break;
            case 4: 
                ListarContactos();
                break;
            case 5: 
                BuscarContacto();
                break;
            case 6: 
                GuardarContactosEnArchivo();
                Console.Clear();
                Console.WriteLine("\nContenido guardado:");
                Console.WriteLine(File.ReadAllText("agenda.csv"));
                Console.WriteLine("Presione Enter para salir...");
                Console.ReadLine();
                DibujarEncabezado("SALIENDO DEL PROGRAMA");
                Console.WriteLine("|                                          |");
                Console.WriteLine("|  Todos los contactos han sido guardados  |");
                Console.WriteLine("|  en agenda.csv                          |");
                Console.WriteLine("|                                          |");
                Console.WriteLine("============================================");
                Thread.Sleep(2000);
                return;
            default: 
                MostrarError("Opción no válida. Por favor ingrese un número del 1 al 6.");
                break;
        }
    }
    else
    {
        MostrarError("Entrada inválida. Debe ingresar un número.");
    }
    
    if (opcion >= 1 && opcion <= 5)
    {
        Console.WriteLine("\n  Presione cualquier tecla para volver al menú...");
        Console.ReadKey();
    }
}