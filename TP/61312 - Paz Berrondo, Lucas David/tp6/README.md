# TP6: Tienda Online - Desarrollo Full Stack
**Blazor WebAssembly + Minimal API + Entity Framework Core + SQLite**

---

**Estado actual**: 🟢 Commit 5 completado - Endpoints de carrito implementados (crear, obtener, vaciar)# 🎯 **OBJETIVO**
Desarrollar una aplicación web completa de tienda online que demuestre dominio de:
- **Frontend**: Blazor WebAssembly 
- **Backend**: Minimal API en C#
- **Persistencia**: Entity Framework Core con SQLite

---

## 📋 **REQUISITOS FUNCIONALES**

### **Frontend - Blazor WASM:**
1. **Catálogo de productos**
   - Listado de productos disponible y buscable
   - Cabecera fija con logo (home), buscador e ícono de carrito con contador
   - Tarjetas con imagen, nombre, descripción, stock y precio
   - Botón "Agregar al carrito" (valida stock) → redirige al carrito

2. **Carrito de compra**
   - Lista de productos, unidades, precio unitario e importe
   - Controles +/- para modificar cantidad (ajusta stock en tiempo real)
   - Botones "Vaciar carrito" y "Confirmar compra" → redirige a confirmación

3. **Confirmación de compra**
   - Resumen (total ítems e importe)
   - Formulario con Nombre, Apellido y Email (obligatorios)
   - Botón "Confirmar" registra compra, limpia carrito → vuelve al catálogo

### **Backend - Minimal API:**
- `GET /productos` (+ búsqueda por query)
- `POST /carritos` (inicializa el carrito)
- `GET /carritos/{carrito}` → Trae los ítems del carrito
- `DELETE /carritos/{carrito}` → Vacía el carrito
- `PUT /carritos/{carrito}/confirmar` (detalle + datos cliente)
- `PUT /carritos/{carrito}/{producto}` → Agrega/actualiza producto en carrito
- `DELETE /carritos/{carrito}/{producto}` → Elimina/reduce producto del carrito

### **Modelo de Datos:**
- **Productos**: Id, Nombre, Descripción, Precio, Stock, ImagenUrl
- **Compras**: Id, Fecha, Total, NombreCliente, ApellidoCliente, EmailCliente
- **Items de compra**: Id, ProductoId, CompraId, Cantidad, PrecioUnitario
- **Datos iniciales**: Al menos 10 productos consistentes con imágenes

---

## 🚀 **PLAN DE COMMITS (Mínimo 10)**

### **✅ COMPLETADOS:**
- [x] **Commit 1**: Creación de modelos de datos (Producto, Compra, ItemCompra, Carrito + DTOs)
- [x] **Commit 2**: Configuración de Entity Framework y DbContext
- [x] **Commit 3**: Implementación de datos iniciales (Seeding) - 10 productos de tecnología
- [x] **Commit 4**: Implementación de endpoints de productos (GET /productos)
- [x] **Commit 5**: Implementación de endpoints de carrito (POST, GET, DELETE)
- [x] **Commit 6**: Implementación de endpoints de items de carrito (PUT, DELETE)
- [ ] **Commit 7**: Actualización de ApiService en cliente para nuevos endpoints
- [ ] **Commit 8**: Implementación de página de catálogo de productos
- [ ] **Commit 9**: Implementación de página de carrito de compra
- [ ] **Commit 10**: Implementación de página de confirmación de compra
- [ ] **Commit 11**: Implementación de navegación y header con buscador
- [ ] **Commit 12**: Mejoras de UI/UX y validaciones
- [ ] **Commit 13**: Testing final y documentación de código

### **📝 NOTAS DE DESARROLLO:**
- **Simplicidad**: Código claro y bien documentado para defensa oral
- **Consistencia**: Productos temáticos (ej: celulares, accesorios)
- **Validaciones**: Stock en tiempo real
- **Bootstrap**: Disponible pero no obligatorio
- **Imágenes**: URLs representativas para productos

---

## 🛠️ **ESTRUCTURA DEL PROYECTO**

```
tp6/
├── cliente/                 # Blazor WebAssembly
│   ├── Pages/
│   │   ├── Home.razor      # Catálogo de productos
│   │   ├── Carrito.razor   # Carrito de compra
│   │   └── Compra.razor    # Confirmación de compra
│   ├── Services/
│   │   └── ApiService.cs   # Servicios HTTP
│   └── Shared/
│       └── Header.razor    # Navegación y buscador
└── servidor/               # ASP.NET Core Minimal API
    ├── Models/             # Modelos de datos
    ├── Data/               # DbContext y configuración EF
    └── Program.cs          # Endpoints de la API
```

---

## 🔧 **COMANDOS DE EJECUCIÓN**
```bash
# Terminal 1 - Servidor
cd servidor
dotnet run

# Terminal 2 - Cliente  
cd cliente
dotnet run
```

---

## 📅 **CRONOGRAMA**
- **Fecha límite**: Sábado 14 de junio a las 23:59 hs
- **Entrega**: Pull request con legajo, nombre y apellido
- **Defensa**: Explicación oral línea por línea del código

---

## 🎓 **CRITERIOS DE EVALUACIÓN**
- ✅ Funcionalidad completa según requisitos
- ✅ Código bien documentado y defendible
- ✅ Mínimo 10 commits descriptivos
- ✅ Arquitectura correcta (Frontend/Backend/BD)
- ✅ Validaciones y manejo de errores
- ✅ Interfaz de usuario funcional

---

**Estado actual**: � Commit 1 completado - Modelos de datos implementados

---

## 📝 **DETALLES DE COMMITS COMPLETADOS**

### **✅ Commit 1: Creación de modelos de datos**
**Archivos creados:**
- `servidor/Models/Producto.cs` - Modelo principal de productos
- `servidor/Models/Compra.cs` - Modelo de compras confirmadas  
- `servidor/Models/ItemCompra.cs` - Items individuales de compras
- `servidor/Models/Carrito.cs` - Carrito temporal y sus items
- `servidor/DTOs/TiendaDTOs.cs` - DTOs para comunicación API

**Funcionalidad implementada:**
- ✅ Modelos con documentación completa
- ✅ Propiedades calculadas (Subtotal, Total)
- ✅ Relaciones navegacionales entre entidades
- ✅ DTOs para transferencia de datos cliente-servidor
- ✅ Compilación exitosa verificada

**Próximo paso**: Implementación de datos iniciales (Seeding) - 10 productos

### **✅ Commit 2: Configuración de Entity Framework y DbContext**
**Archivos creados/modificados:**
- `servidor/Data/TiendaContext.cs` - DbContext principal con configuraciones
- `servidor/appsettings.json` - Cadena de conexión SQLite
- `servidor/appsettings.Development.json` - Config desarrollo
- `servidor/Program.cs` - Registro de servicios EF y creación de BD

**Funcionalidad implementada:**
- ✅ DbContext con configuración completa de entidades
- ✅ Relaciones entre tablas (FK, índices, restricciones)
- ✅ Cadena de conexión SQLite configurada
- ✅ Creación automática de base de datos
- ✅ Verificación exitosa: servidor ejecutándose y BD creada

**Próximo paso**: Implementación de endpoints de productos (GET /productos)

### **✅ Commit 3: Implementación de datos iniciales (Seeding)**
**Archivos creados/modificados:**
- `servidor/Services/DatabaseSeeder.cs` - Servicio de seeding con 10 productos
- `servidor/Program.cs` - Integración del seeding al iniciar la aplicación

**Funcionalidad implementada:**
- ✅ Servicio DatabaseSeeder con 10 productos de tecnología consistentes
- ✅ Productos con datos realistas: iPhone, Samsung, MacBook, iPad, etc.
- ✅ Imágenes representativas usando URLs de Unsplash
- ✅ Verificación que no se dupliquen datos en ejecuciones posteriores
- ✅ Seeding automático al iniciar la aplicación
- ✅ Verificación exitosa: 10 productos insertados correctamente

**Productos incluidos:**
1. iPhone 15 Pro ($1299.99, Stock: 15)
2. Samsung Galaxy S24 Ultra ($1199.99, Stock: 12)  
3. MacBook Air M3 ($1099.99, Stock: 8)
4. AirPods Pro 2 ($249.99, Stock: 25)
5. iPad Pro 11" ($799.99, Stock: 10)
6. Apple Watch Series 9 ($399.99, Stock: 18)
7. Sony WH-1000XM5 ($399.99, Stock: 14)
8. Nintendo Switch OLED ($349.99, Stock: 20)
9. Logitech MX Master 3S ($99.99, Stock: 30)
10. Samsung 4K Monitor 27" ($329.99, Stock: 6)

**Próximo paso**: Implementación de endpoints de carrito (POST, GET, DELETE)

### **✅ Commit 4: Implementación de endpoints de productos**
**Archivos modificados:**
- `servidor/Program.cs` - Implementación de endpoints GET /api/productos

**Funcionalidad implementada:**
- ✅ **GET /api/productos** - Obtiene todos los productos con mapeo a DTOs
- ✅ **GET /api/productos?buscar=término** - Búsqueda por nombre (case-insensitive)
- ✅ **GET /api/productos/{id}** - Obtiene producto específico por ID
- ✅ **Manejo de errores**: 404 para IDs inexistentes, 500 para errores del servidor
- ✅ **Documentación**: XML comments y metadata de endpoints
- ✅ **Validaciones**: Verificación de existencia y parámetros válidos
- ✅ **Logging**: Errores registrados en consola para debugging

**Endpoints probados exitosamente:**
- ✅ Listado completo (10 productos retornados)
- ✅ Búsqueda "apple" (1 resultado: Apple Watch)
- ✅ Búsqueda "samsung" (2 resultados: Galaxy y Monitor)
- ✅ Producto por ID válido (iPhone 15 Pro)
- ✅ Error 404 para ID inexistente (999)

**Características técnicas:**
- **Mapeo automático**: Entidades a DTOs para respuestas limpias
- **Consultas optimizadas**: LINQ con Entity Framework
- **Búsqueda flexible**: Contiene + case-insensitive
- **Metadata OpenAPI**: WithName, WithSummary, WithDescription

**Próximo paso**: Implementación de endpoints de items de carrito (agregar/actualizar/eliminar productos)

### **✅ Commit 5: Implementación de endpoints de carrito**
**Archivos creados/modificados:**
- `servidor/Services/CarritoService.cs` - Servicio para manejar carritos en memoria
- `servidor/Program.cs` - Registro del servicio y endpoints de carrito

**Funcionalidad implementada:**
- ✅ **POST /api/carritos** - Crea carrito vacío con ID único (GUID)
- ✅ **GET /api/carritos/{id}** - Obtiene contenido del carrito con precios actualizados
- ✅ **DELETE /api/carritos/{id}** - Vacía carrito eliminando todos los items
- ✅ **GET /api/carritos/estadisticas** - Endpoint de debugging para monitoreo
- ✅ **CarritoService**: Gestión completa de carritos temporales en memoria
- ✅ **Validaciones**: Verificación de existencia de carritos
- ✅ **Manejo de errores**: 404 para carritos inexistentes, 500 para errores del servidor
- ✅ **Logging**: Actividad de carritos registrada en consola

**Endpoints probados exitosamente:**
- ✅ Creación de carrito (GUID: 4d020c38-245b-4078-8739-ced30ba3d6fa)
- ✅ Obtener carrito vacío (0 items, total: 0)
- ✅ Estadísticas (2 carritos activos monitoreados)
- ✅ Vaciar carrito (confirmación con timestamp)
- ✅ Error 404 para carrito inexistente

**Características técnicas del CarritoService:**
- **Almacenamiento en memoria**: Dictionary<string, Carrito> para sesiones temporales
- **IDs únicos**: Generación automática de GUIDs para identificar carritos
- **Limpieza automática**: Método para eliminar carritos antiguos (>24 horas)
- **Conversión a DTOs**: Mapeo automático para respuestas del API
- **Precios actualizados**: Consulta en tiempo real a la BD para precios actuales
- **Estadísticas**: Monitoreo de carritos activos, items totales y valor total

**Próximo paso**: Implementación de endpoints para confirmación de compra y persistencia en BD

### **✅ Commit 6: Implementación de endpoints PUT/DELETE para gestión de items en carrito**
**Archivos creados/modificados:**
- `servidor/Program.cs` - Nuevos endpoints PUT/DELETE para items de carrito
- `servidor/DTOs/TiendaDTOs.cs` - Agregado ActualizarItemCarritoDto
- `servidor/Services/CarritoService.cs` - Corrección de lógica para reemplazar cantidad
- `servidor/test-endpoints-carrito.http` - Archivo de pruebas exhaustivas

**Funcionalidad implementada:**
- ✅ **PUT /api/carritos/{carritoId}/{productoId}** - Agregar/actualizar producto en carrito
- ✅ **DELETE /api/carritos/{carritoId}/{productoId}** - Eliminar producto específico del carrito
- ✅ **ActualizarItemCarritoDto** - DTO para enviar cantidad en requests PUT
- ✅ **Validación de stock** - Verificar disponibilidad antes de agregar productos
- ✅ **Validación de cantidades** - Rechazar cantidades ≤ 0 con error 400
- ✅ **Manejo de productos inexistentes** - Error 404 para productos no encontrados
- ✅ **Manejo de carritos inexistentes** - Error 404 para carritos no válidos
- ✅ **Eliminación completa** - DELETE remueve producto independiente de cantidad

**Correcciones implementadas:**
- ✅ **AgregarProductoAsync**: Cambiado de sumar a reemplazar cantidad (comportamiento PUT correcto)
- ✅ **EliminarProductoCompletoAsync**: Usado en endpoint DELETE para eliminación total
- ✅ **Validación de stock**: Verificar contra stock actual, no cantidad previa

**Endpoints probados exitosamente:**
- ✅ PUT agregar producto nuevo (iPhone 15 Pro × 2) ✅
- ✅ PUT actualizar cantidad (iPhone 15 Pro × 5, no × 7) ✅  
- ✅ PUT agregar segundo producto (Samsung Galaxy × 1) ✅
- ✅ DELETE eliminar producto específico completamente ✅
- ✅ Error 400 para cantidad negativa (-1) ✅
- ✅ Error 400 para cantidad cero (0) ✅
- ✅ Error 404 para producto inexistente (ID 999) ✅
- ✅ Error 404 para eliminar producto no en carrito ✅

**Características técnicas:**
- **Comportamiento REST correcto**: PUT reemplaza/establece, POST agrega, DELETE elimina
- **Validaciones robustas**: Stock, existencia de productos/carritos, cantidades válidas
- **Códigos HTTP apropiados**: 200 OK, 400 Bad Request, 404 Not Found, 500 Internal Error
- **Mensajes descriptivos**: Respuestas JSON con detalles del error y contexto
- **Logging detallado**: Actividad de carritos registrada para debugging
- **Pruebas exhaustivas**: 15 casos de prueba cubriendo éxito y errores

**Próximo paso**: Implementación de endpoints de confirmación de compra y registro en base de datos
