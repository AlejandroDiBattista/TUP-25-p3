# API REST Agenda de Contactos

Este proyecto implementa una API REST para gestionar una agenda de contactos usando C#, Minimal API, Entity Framework Core y SQLite.

## Características
- CRUD de contactos (nombre, apellido, teléfono, email)
- 10 contactos iniciales precargados (sin migraciones)
- Almacenamiento en SQLite

## Uso rápido

1. Instala las dependencias:
   ```sh
   dotnet restore
   ```
2. Ejecuta la aplicación:
   ```sh
   dotnet run
   ```

La API estará disponible en `http://localhost:5000` o el puerto configurado.

## Estructura principal
- `Program.cs`: Configuración de la API y endpoints
- `Contact.cs`: Modelo de datos
- `ContactsDbContext.cs`: Contexto de EF Core

## Notas
- No se usan migraciones, la base de datos y los datos iniciales se crean automáticamente al iniciar.
- Puedes modificar o ampliar los contactos iniciales en el código fuente.
