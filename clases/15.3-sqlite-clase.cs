// #r "nuget: Microsoft.Data.Sqlite, 9.0.4"
#r "nuget: Microsoft.Data.SqlClient, 9.0.4"

// using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;

// SQLitePCL.Batteries_V2.Init();

// const string connectionString = "Data Source=contactos.db";
const string connectionString = "Server=localhost;Database=ContactosDb;Trusted_Connection=True;TrustServerCertificate=True;";

public class Contacto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }

    public string NombreCompleto => $"{Apellido}, {Nombre}";
    public override string ToString() => $"{Id,2} {NombreCompleto, -30}  {Telefono}";
}

public void CrearTablaContacto() {
    // var conexion = new SqliteConnection(connectionString);
    using var connection = new SqlConnection(connectionString);

    conexion.Open();

    // var consulta = """
    //     CREATE TABLE IF NOT EXISTS contacto (
    //         id INTEGER PRIMARY KEY AUTOINCREMENT,
    //         nombre TEXT NOT NULL,
    //         apellido TEXT NOT NULL,
    //         telefono TEXT NOT NULL
    //     )
    // """;
    
    var query = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='contacto' AND xtype='U')
        CREATE TABLE contacto (
            id INT IDENTITY(1,1) PRIMARY KEY,
            nombre NVARCHAR(100) NOT NULL,
            apellido NVARCHAR(100) NOT NULL,
            telefono NVARCHAR(50) NOT NULL
        )";

    using var comando = new SqlCommand(consulta, conexion);
    comando.ExecuteNonQuery();
    comando.Dispose();

    conexion.Dispose();
}

public List<Contacto> ObtenerContactos() {
    var contactos = new List<Contacto>();
    using (var conexion = new SqliteConnection(connectionString)) {
        conexion.Open();

        var consulta = "SELECT id, nombre, apellido, telefono FROM contacto";
        using (var comando = new SqliteCommand(consulta, conexion)) {
            using (var lector = comando.ExecuteReader()) {
                while (lector.Read()) {
                    var contacto = new Contacto {
                        Id       = lector.GetInt32(0),
                        Nombre   = lector.GetString(1),
                        Apellido = lector.GetString(2),
                        Telefono = lector.GetString(3)
                    };
                    contactos.Add(contacto);
                }
            }
        }
    }
    
    return contactos;
}


public void InsertarContacto(Contacto c) {
    using var conexion = new SqliteConnection(connectionString);
    conexion.Open();

    var consulta = "INSERT INTO contacto (nombre, apellido, telefono) VALUES (@nombre, @apellido, @telefono)";

    using var comando = new SqliteCommand(consulta, conexion);
    comando.Parameters.AddWithValue("@nombre", c.Nombre);
    comando.Parameters.AddWithValue("@apellido", c.Apellido);
    comando.Parameters.AddWithValue("@telefono", c.Telefono);
    comando.ExecuteNonQuery();
}

// Inserta usando reflexión (excepto Id)
public void Insertar<T>(T c) {
    using var conexion = new SqliteConnection(connectionString);
    conexion.Open();

    var tipo = typeof(T);
    var props = tipo.GetProperties()
        .Where(p => p.Name != "Id") // Excluir Id (autoincremental)
        .ToArray();
    var columnas   = string.Join(", ", props.Select(p => p.Name.ToLower()));
    var parametros = string.Join(", ", props.Select(p => "@" + p.Name.ToLower()));

    var consulta = $"INSERT INTO {tipo.Name} ({columnas}) VALUES ({parametros})";
    using var comando = new SqliteCommand(consulta, conexion);

    foreach (var prop in props) {
        var valor = prop.GetValue(c) ?? DBNull.Value;
        comando.Parameters.AddWithValue("@" + prop.Name.ToLower(), valor);
    }

    comando.ExecuteNonQuery();
}


public void ActualizarContacto(Contacto c) {
    using var conexion = new SqliteConnection(connectionString);
    conexion.Open();

    var consulta = "UPDATE contacto SET nombre = @nombre, apellido = @apellido, telefono = @telefono WHERE id = @id";
    using var comando = new SqliteCommand(consulta, conexion);
    comando.Parameters.AddWithValue("@nombre", c.Nombre);
    comando.Parameters.AddWithValue("@apellido", c.Apellido);
    comando.Parameters.AddWithValue("@telefono", c.Telefono);
    comando.Parameters.AddWithValue("@id", c.Id);
    comando.ExecuteNonQuery();

}

// Ejemplo de uso:

var ale = new Contacto { Id = 1, Nombre = "Alejandro", Apellido = "Gonzalez", Telefono = "(123) 456-7890" };
var ana = new Contacto { Id = 2, Nombre = "Ana",       Apellido = "Lopez",    Telefono = "(987) 654-3210" };

CrearTablaContacto();

InsertarContacto(ale);
InsertarContacto(ana);

foreach (var c in ObtenerContactos()) {
    Console.WriteLine(c.ToString());
}

Console.Clear();
var tipo = typeof(Contacto);
Console.WriteLine($"Tipo: {tipo.Name}");

Console.WriteLine($"Propiedades: {tipo.GetProperties().Length}");
foreach (var prop in tipo.GetProperties()) {
    Console.WriteLine($"Propiedad: {prop.Name} - Tipo: {prop.PropertyType.Name}");
}

Console.WriteLine($"Campos: {tipo.GetFields().Length}");
foreach (var field in tipo.GetFields()) {
    Console.WriteLine($"Campo: {field.Name} - Tipo: {field.FieldType.Name}");
}

Console.WriteLine($"Métodos: {tipo.GetMethods().Length}");
foreach (var method in tipo.GetMethods()) {
    Console.WriteLine($"Método: {method.Name} - Tipo: {method.ReturnType.Name}");
}