# Descripción de la API REST - Agenda de Contactos
- Ubicacion servidor: http://localhost:5198

Esta API permite gestionar una agenda de contactos mediante operaciones CRUD. Está desarrollada en C# usando Minimal API, Entity Framework Core y SQLite.

## Endpoints

### Obtener todos los contactos
- **GET** `/contacts`
- **Respuesta:** 200 OK
- **Cuerpo:** Lista de contactos

### Obtener un contacto por ID
- **GET** `/contacts/{id}`
- **Respuesta:** 200 OK (si existe), 404 Not Found (si no existe)
- **Cuerpo:** Objeto contacto

### Crear un nuevo contacto
- **POST** `/contacts`
- **Cuerpo de solicitud:**
  ```json
  {
    "nombre": "string",
    "apellido": "string",
    "telefono": "string",
    "email": "string"
  }
  ```
- **Respuesta:** 201 Created (con el contacto creado)

### Actualizar un contacto existente
- **PUT** `/contacts/{id}`
- **Cuerpo de solicitud:** Igual que POST
- **Respuesta:** 204 No Content (si se actualizó), 404 Not Found (si no existe)

### Eliminar un contacto
- **DELETE** `/contacts/{id}`
- **Respuesta:** 204 No Content (si se eliminó), 404 Not Found (si no existe)

## Modelo de Contacto
```json
{
  "id": int,
  "nombre": "string",
  "apellido": "string",
  "telefono": "string",
  "email": "string"
}
```

## Notas
- Todos los endpoints aceptan y devuelven JSON.
- La base de datos se inicializa con 10 contactos de ejemplo.
- No requiere autenticación.
