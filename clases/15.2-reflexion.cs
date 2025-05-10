public class Contacto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }
    public int Edad = 0;

    public string NombreCompleto => $"{Apellido}, {Nombre}";
    public override string ToString() => $"{Id,2} {NombreCompleto, -30}  {Telefono}";
}


Console.Clear();
var tipo = typeof(Contacto);
Console.WriteLine($"Tipo: {tipo.Name}");

Console.WriteLine($"\nPropiedades: {tipo.GetProperties().Length}");
foreach (var prop in tipo.GetProperties()) {
    Console.WriteLine($" {prop.Name,-20} ({prop.PropertyType.Name})");
}

Console.WriteLine($"\nCampos: {tipo.GetFields().Length}");
foreach (var field in tipo.GetFields()) {
    Console.WriteLine($" {field.Name,-20} ({field.FieldType.Name})");
}

Console.WriteLine($"\nMétodos: {tipo.GetMethods().Length}");
foreach (var method in tipo.GetMethods()) {
    Console.WriteLine($" {method.Name,-20} ({method.ReturnType.Name})");
}