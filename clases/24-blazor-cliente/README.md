# Gestión de Contactos - Blazor WebAssembly

Sistema completo de gestión de contactos desarrollado en Blazor WebAssembly con operaciones CRUD conectado a una API REST.

## 📋 Características

- ✅ **Crear contactos** - Agregar nuevos contactos con validación completa
- ✅ **Listar contactos** - Visualizar todos los contactos en una tabla responsiva
- ✅ **Editar contactos** - Modificar información existente
- ✅ **Eliminar contactos** - Eliminar contactos con confirmación de seguridad
- ✅ **Validación de datos** - Validaciones en tiempo real con Data Annotations
- ✅ **Interfaz moderna** - UI responsiva con Bootstrap 5 y Bootstrap Icons
- ✅ **Manejo de errores** - Gestión completa de errores y estados de carga

## 🏗️ Arquitectura

### Modelos
- **`Contacto`** - Modelo principal con validaciones
- **`ApiResponse<T>`** - Clase genérica para respuestas de API

### Servicios
- **`IContactoService`** - Interfaz del servicio CRUD
- **`ContactoService`** - Implementación del servicio con HttpClient

### Configuración
- **`ApiSettings`** - Configuración centralizada de la API
- **`appsettings.json`** - Configuración de URL base y endpoints

## 🚀 Instalación y Configuración

### 1. Clonar y restaurar dependencias
```bash
dotnet restore
```

### 2. Configurar la API
Edita el archivo `wwwroot/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://tu-api.com/",
    "ContactosEndpoint": "api/contactos",
    "TimeoutSeconds": 30
  }
}
```

### 3. Ejecutar la aplicación
```bash
dotnet run
```

## 🔌 Configuración de API REST

El servicio espera que tu API REST implemente los siguientes endpoints:

### Endpoints Requeridos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/contactos` | Obtener todos los contactos |
| `GET` | `/api/contactos/{id}` | Obtener un contacto por ID |
| `POST` | `/api/contactos` | Crear un nuevo contacto |
| `PUT` | `/api/contactos/{id}` | Actualizar un contacto existente |
| `DELETE` | `/api/contactos/{id}` | Eliminar un contacto |

### Modelo de Datos

Tu API debe manejar el siguiente modelo JSON:

```json
{
  "id": 1,
  "nombre": "Juan",
  "apellido": "Pérez",
  "telefono": "+54 11 1234-5678",
  "email": "juan.perez@email.com",
  "edad": 30
}
```

### Respuestas Esperadas

#### GET /api/contactos
```json
[
  {
    "id": 1,
    "nombre": "Juan",
    "apellido": "Pérez",
    "telefono": "+54 11 1234-5678",
    "email": "juan.perez@email.com",
    "edad": 30
  }
]
```

#### POST /api/contactos
**Request Body:**
```json
{
  "nombre": "María",
  "apellido": "García",
  "telefono": "+54 11 9876-5432",
  "email": "maria.garcia@email.com",
  "edad": 25
}
```

**Response:**
```json
{
  "id": 2,
  "nombre": "María",
  "apellido": "García",
  "telefono": "+54 11 9876-5432",
  "email": "maria.garcia@email.com",
  "edad": 25
}
```

## 📱 Uso de la Aplicación

### Página Principal
- Información general del sistema
- Enlaces rápidos a la gestión de contactos
- Documentación de configuración

### Gestión de Contactos
- **Lista de contactos**: Tabla con todos los contactos
- **Formulario de creación**: Modal para agregar nuevos contactos
- **Edición inline**: Editar contactos existentes
- **Confirmación de eliminación**: Diálogo de confirmación antes de eliminar

## 🛠️ Desarrollo

### Estructura del Proyecto
```
├── Models/
│   ├── Contacto.cs
│   └── ApiResponse.cs
├── Services/
│   ├── IContactoService.cs
│   └── ContactoService.cs
├── Configuration/
│   └── ApiSettings.cs
├── Pages/
│   ├── Home.razor
│   └── Contactos.razor
├── Layout/
│   ├── MainLayout.razor
│   └── NavMenu.razor
└── wwwroot/
    └── appsettings.json
```

### Validaciones Implementadas

El modelo `Contacto` incluye las siguientes validaciones:

- **Nombre**: Requerido, máximo 50 caracteres
- **Apellido**: Requerido, máximo 50 caracteres  
- **Teléfono**: Requerido, formato de teléfono válido, máximo 20 caracteres
- **Email**: Requerido, formato de email válido, máximo 100 caracteres
- **Edad**: Requerida, entre 1 y 120 años

### Tecnologías Utilizadas

- **Blazor WebAssembly** - Framework frontend
- **Bootstrap 5** - Framework CSS
- **Bootstrap Icons** - Iconografía
- **HttpClient** - Cliente HTTP para API REST
- **Data Annotations** - Validaciones de modelo

## 🚨 Manejo de Errores

El sistema incluye manejo completo de errores:

- **Errores de conexión**: Mostrados al usuario con opción de reintento
- **Errores de validación**: Mostrados en tiempo real en el formulario
- **Estados de carga**: Indicadores visuales durante operaciones
- **Confirmaciones**: Diálogos de confirmación para operaciones críticas

## 🔧 Personalización

### Cambiar URL de API
Modifica `wwwroot/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://nueva-api.com/",
    "ContactosEndpoint": "api/mis-contactos",
    "TimeoutSeconds": 60
  }
}
```

### Agregar Nuevos Campos
1. Actualiza el modelo `Contacto.cs`
2. Agrega validaciones con Data Annotations
3. Actualiza el formulario en `Contactos.razor`
4. Actualiza la tabla de visualización

## 🔧 Ejemplo de Controlador Backend (Referencia)

Si necesitas crear el backend, aquí tienes un ejemplo de controlador ASP.NET Core:

```csharp
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactosController : ControllerBase {
    private readonly IContactoRepository _repository;

    public ContactosController(IContactoRepository repository) {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contacto>>> ObtenerTodos() {
        try {
            var contactos = await _repository.ObtenerTodosAsync();
            return Ok(contactos);
        } catch (Exception ex) {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contacto>> ObtenerPorId(int id) {
        try {
            var contacto = await _repository.ObtenerPorIdAsync(id);
            
            if (contacto == null) {
                return NotFound(new { message = $"Contacto con ID {id} no encontrado" });
            }
            
            return Ok(contacto);
        } catch (Exception ex) {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Contacto>> Crear([FromBody] Contacto contacto) {
        try {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var contactoCreado = await _repository.CrearAsync(contacto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = contactoCreado.Id }, contactoCreado);
        } catch (Exception ex) {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Contacto>> Actualizar(int id, [FromBody] Contacto contacto) {
        try {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != contacto.Id) {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del contacto" });
            }

            var contactoExistente = await _repository.ObtenerPorIdAsync(id);
            if (contactoExistente == null) {
                return NotFound(new { message = $"Contacto con ID {id} no encontrado" });
            }

            var contactoActualizado = await _repository.ActualizarAsync(contacto);
            return Ok(contactoActualizado);
        } catch (Exception ex) {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(int id) {
        try {
            var contactoExistente = await _repository.ObtenerPorIdAsync(id);
            if (contactoExistente == null) {
                return NotFound(new { message = $"Contacto con ID {id} no encontrado" });
            }

            await _repository.EliminarAsync(id);
            return NoContent();
        } catch (Exception ex) {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}

// Interfaz del repositorio
public interface IContactoRepository {
    Task<IEnumerable<Contacto>> ObtenerTodosAsync();
    Task<Contacto?> ObtenerPorIdAsync(int id);
    Task<Contacto> CrearAsync(Contacto contacto);
    Task<Contacto> ActualizarAsync(Contacto contacto);
    Task<bool> EliminarAsync(int id);
}
```

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo LICENSE para más detalles.
