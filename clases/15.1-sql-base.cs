#r "nuget: Microsoft.Data.Sqlite, 9.0.4" 

using Microsoft.Data.Sqlite;

SQLitePCL.Batteries_V2.Init(); // Inicializar SQLite PCL

const string cadenaConexion = "Data Source=contactos.db"; // Cadena de conexión a la base de datos SQLite

void CrearTablaContactos() {
    var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();

    var consulta = """
        CREATE TABLE IF NOT EXISTS contacto (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre TEXT NOT NULL,
            apellido TEXT NOT NULL,
            telefono TEXT NOT NULL
        )
    """;

    var comando = new SqliteCommand(consulta, conexion);
    comando.ExecuteNonQuery();
    comando.Dispose();

    conexion.Dispose();
}

void InsertarContacto(string nombre, string apellido, string telefono) {
    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();

    var consulta = """
        INSERT INTO contacto 
            (nombre, apellido, telefono) 
            VALUES (@nombre, @apellido, @telefono)
    """;
    
    using var comando = new SqliteCommand(consulta, conexion);
    comando.Parameters.AddWithValue("@nombre", nombre);
    comando.Parameters.AddWithValue("@apellido", apellido);
    comando.Parameters.AddWithValue("@telefono", telefono);
    comando.ExecuteNonQuery();
}

void ListarContactos() {
    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();

    var consulta = "SELECT id, nombre, apellido, telefono FROM contacto";
    using var comando = new SqliteCommand(consulta, conexion);
    using var lector = comando.ExecuteReader();
    while (lector.Read()) {
        var contacto = new {
            Id       = lector.GetInt32(0),
            Nombre   = lector.GetString(1),
            Apellido = lector.GetString(2),
            Telefono = lector.GetString(3)
        };
        Console.WriteLine(contacto);
    }
}


// Crear la tabla si no existe
CrearTablaContactos();

InsertarContacto("Juan", "Pérez",    "(123) 456-789");
InsertarContacto("Ana",  "Gómez",    "(987) 654-321");
InsertarContacto("Luis", "Martínez", "(456) 789-123");

ListarContactos();