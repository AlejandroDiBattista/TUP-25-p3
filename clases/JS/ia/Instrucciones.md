# Instrucciones para Construir la Agenda de Contactos Fullstack

## 1. Backend (carpeta `servidor`)

- Crea un proyecto C# Minimal API.
- Usa Entity Framework Core con SQLite.
- Define el modelo `Contacto` con: `id`, `nombre`, `apellido`, `telefono`, `email`.
- Implementa endpoints RESTful:
  - `GET /contacts` (todos)
  - `GET /contacts/{id}` (por id)
  - `POST /contacts` (crear)
  - `PUT /contacts/{id}` (actualizar)
  - `DELETE /contacts/{id}` (eliminar)
- Inicializa la base de datos con 10 contactos de ejemplo (sin migraciones).
- Habilita CORS para permitir peticiones desde `http://localhost:5173`.
- Registra el contexto de EF Core usando `AddDbContext` y configura SQLite.
- El constructor de tu `DbContext` debe aceptar `DbContextOptions` y pasarlo a la base.
- Ejemplo de configuración de CORS y EF Core en `Program.cs`:

```csharp
builder.Services.AddDbContext<ContactsDbContext>(options => {
    options.UseSqlite("Data Source=contacts.db");
});
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();
app.UseCors("AllowFrontend");
```

---

## 2. Frontend (carpeta `cliente`)

- Crea el proyecto con Vite y React (JavaScript).
- Extrae toda la comunicación con la API en un módulo de servicio (`src/contactService.js`).
  - Funciones: `getContacts`, `getContact`, `createContact`, `updateContact`, `deleteContact`.
- Usa componentes para la UI:
  - `App.jsx`: lógica principal y estado.
  - `ContactTable.jsx`: muestra la tabla de contactos.
  - `ContactForm.jsx`: formulario para crear/editar.
- El componente principal (`App.jsx`) debe importar y usar el módulo de servicio para todas las operaciones.
- El diseño debe ser minimalista y elegante, con código claro y simple.
- Ejemplo de uso del módulo de servicio en `App.jsx`:

```javascript
import { getContacts, createContact, updateContact, deleteContact } from './contactService';
// ...lógica de estado y handlers usando estas funciones...
```

---

## 3. Consideraciones adicionales

- Todos los endpoints aceptan y devuelven JSON.
- No requiere autenticación.
- El frontend debe mostrar mensajes de error si la API falla.
- El código debe estar modularizado y ser fácil de mantener.

---

Con estas instrucciones puedes recrear la app desde cero siguiendo las mejores prácticas y la arquitectura actual.
