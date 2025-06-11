# TP6: Tienda Online - Desarrollo Full Stack
**Blazor WebAssembly + Minimal API + Entity Framework Core + SQLite**

---

**Estado actual**: 🟢 Commit 3 completado - Datos iniciales implementados (10 productos de tecnología)# 🎯 **OBJETIVO**
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
- [ ] **Commit 4**: Implementación de endpoints de productos (GET /productos)
- [ ] **Commit 5**: Implementación de endpoints de carrito (POST, GET, DELETE)
- [ ] **Commit 6**: Implementación de endpoints de items de carrito (PUT, DELETE)
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

**Próximo paso**: Implementación de endpoints de productos (GET /productos con búsqueda)
