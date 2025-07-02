#r "nuget: Microsoft.Data.SqlClient, 6.0.2"

using Microsoft.Data.SqlClient;

// Cambia la cadena de conexión según tu entorno SQL Server
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
    using var conexion = new SqlConnection(connectionString);
    conexion.Open();
    var consulta = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='contacto' AND xtype='U')
        CREATE TABLE contacto (
            id INT IDENTITY(1,1) PRIMARY KEY,
            nombre NVARCHAR(100) NOT NULL,
            apellido NVARCHAR(100) NOT NULL,
            telefono NVARCHAR(50) NOT NULL
        )";
    using var comando = new SqlCommand(consulta, conexion);
    comando.ExecuteNonQuery();
    conexion.Close();
}

public List<Contacto> ObtenerContactos() {
    var contactos = new List<Contacto>();
    using var conexion = new SqlConnection(connectionString);
    conexion.Open();
    var consulta = "SELECT id, nombre, apellido, telefono FROM contacto";
    using var comando = new SqlCommand(consulta, conexion);
    using var lector = comando.ExecuteReader();
    while (lector.Read()) {
        var contacto = new Contacto {
            Id       = lector.GetInt32(0),
            Nombre   = lector.GetString(1),
            Apellido = lector.GetString(2),
            Telefono = lector.GetString(3)
        };
        contactos.Add(contacto);
    }
    conexion.Close();
    return contactos;
}

public void InsertarContacto(Contacto c) {
    using (var conexion = new SqlConnection(connectionString)) {
        conexion.Open();
        var consulta = "INSERT INTO contacto (nombre, apellido, telefono) VALUES (@nombre, @apellido, @telefono)";
        using var comando = new SqlCommand(consulta, conexion);
        comando.Parameters.AddWithValue("@nombre", c.Nombre);
        comando.Parameters.AddWithValue("@apellido", c.Apellido);
        comando.Parameters.AddWithValue("@telefono", c.Telefono);
        comando.ExecuteNonQuery();
    }
}

public void ActualizarContacto(Contacto c) {
    using var conexion = new SqlConnection(connectionString);
    conexion.Open();
    var consulta = "UPDATE contacto SET nombre = @nombre, apellido = @apellido, telefono = @telefono WHERE id = @id";
    using var comando = new SqlCommand(consulta, conexion);
    comando.Parameters.AddWithValue("@nombre", c.Nombre);
    comando.Parameters.AddWithValue("@apellido", c.Apellido);
    comando.Parameters.AddWithValue("@telefono", c.Telefono);
    comando.Parameters.AddWithValue("@id", c.Id);
    comando.ExecuteNonQuery();
    conexion.Close();
}

// Ejemplo de uso:
var ale = new Contacto { Id = 1, Nombre = "Alejandro", Apellido = "Gonzalez", Telefono = "(123) 456-7890" };
var ana = new Contacto { Id = 2, Nombre = "Ana",       Apellido = "Lopez",    Telefono = "(987) 654-3210" };

CrearTablaContacto();
InsertarContacto(ale);
InsertarContacto(ana);
foreach (var c in ObtenerContactos()) {
    Console.WriteLine($"Id: {c.Id,2} {c.NombreCompleto, -30}  {c.Telefono}");
}