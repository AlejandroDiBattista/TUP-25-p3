# Dudas Parcial 2

a. Comó muestro las images?

1. En el servidor se debe habilitar el acceso a archivos estáticos.

Se debe poner en `program.cs`:
```csharp
app.UseStaticFiles();
```

2. Luego crear la carpeta `wwwroot` y dentro de esta la carpeta poner las imagenes
    Por ejemplo poner el archivo `img01.png` dentro de `wwwroot/img01.png`.

3. Finalmente, en el HTML se debe hacer referencia a la imagen de la siguiente manera:
```html
    <img src="http://localhost:5184/img01.png" alt="Logo" class="img-fluid mb-4" width="200px" />
```
   En esta caso lo puse en `home.razor`


b. Tengo que reinciar el servidor con cada cambio que hago en el código?

No, no es necesario sin se activa la opción de "Hot Reload" en Visual Studio. Esta opción permite que los cambios en el código se reflejen automáticamente en la aplicación sin necesidad de reiniciar el servidor.

```bash
dotnet watch run
```

c. Como conecto la base de datos?

En el archivo `19.1-apirest.cs` se puede ver un ejemplo de como se conecta la base de datos sqlite.

1. En el sevidor en el archivo `program.cs` se debe agregar el siguiente código:

```csharp
// Conectar la base de datos
builder.Services.AddDbContext<AppDb>(options => options.UseSqlite('datos.db'));

// Asegurarse que se cree la base de datos
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
}

class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> opt) : base(opt) { }
    public DbSet<Contacto> Contactos => Set<Contacto>();

    // Crea la base de datos inicial
    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Contacto>().HasData(
            new Contacto { Id = 1, Nombre = "Juan", Apellido = "Pérez", Telefono = "123456789" },
            new Contacto { Id = 2, Nombre = "Ana", Apellido = "Gómez", Telefono = "987654321" },
            new Contacto { Id = 3, Nombre = "Luis", Apellido = "Martínez", Telefono = "456789123" }
        );
    }
}
```

