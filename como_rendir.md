# Sistema de Tienda Online

**Desarrollo full‑stack con C# — Blazor WASM + Minimal API + EF Core/SQLite**

---

## 1. Objetivo

Diseñar y construir una aplicación web completa (“Tienda Online”) que demuestre dominio de:

* **Frontend**: Blazor WebAssembly para la experiencia de usuario.  
* **Backend**:  Minimal API en C# que exponga un servicio REST.  
* **Persistencia**: Entity Framework Core con SQLite.  

---

## 2. Requisitos funcionales

### 2.1 Frontend – Blazor WASM

1. **Catálogo de productos**  
   * Listado de productos disponible y buscable.  
   * Cabecera fija con logo (home), buscador e ícono de carrito con contador de ítems.  
   * Tarjetas con imagen, nombre, descripción, stock y precio.  
   * Botón **Agregar al carrito** (valida stock) → redirige al carrito.  

2. **Carrito de compra**  
   * Lista de productos, unidades, precio unitario e importe.  
   * Controles +/- para modificar cantidad (ajusta stock en tiempo real).  
   * Botón **Vaciar carrito** (devuelve el stock) → redirige al catálogo.
   * Botón **Confirmar compra** → redirige a la confirmación.

3. **Confirmación de compra**  
   * Resumen (total ítems e importe).  
   * Formulario con Nombre, Apellido y Email (obligatorios).  
   * Botón **Confirmar** registra compra, limpia carrito → vuelve al catálogo.  

### 2.2 Backend – Minimal API

* Funciones del API necesarias:  
  * `GET /productos` (+ búsqueda por query).  

  * `POST /carritos` (inicializa el carrito).
  * `GET /carritos/{carrito}` → Trae los ítems del carrito.  
  * `DELETE /carritos/{carrito}` → Vacía el carrito.
  * `PUT /carritos/{carrito}/confirmar` (detalle + datos cliente).  

  * `PUT /carritos/{carrito}/{producto}` → Agrega un producto al carrito (o actualiza cantidad).  
  * `DELETE /carritos/{carrito}/{producto}` → Elimina un producto del carrito (o reduce cantidad).

* Todas las operaciones deben validar stock y persistir cambios.  

---

### 3. Modelo de datos
  * Productos: `Id`, `Nombre`, `Descripción`, `Precio`, `Stock`, `ImagenUrl`.
  * Compras: `Id`, `Fecha`, `Total`, `NombreCliente`, `ApellidoCliente`, `EmailCliente`.
  * Items de compra: `Id`, `ProductoId`, `CompraId`, `Cantidad`, `PrecioUnitario`.

  El sistema debe cargar al menos 10 productos de ejemplo al iniciar.
  Deben ser registros consistentes (por ejemplo celulares, accesorios, gaseosas, etc.).
  Deben tener imágenes representativas.

  Se debe usar Entity Framework Core para definir el modelo y usar SQLite como base de datos.

### 4. Desarrollo del sistema
* El trabajo es estrictamente individual (no se permite el trabajo en grupo).
* Puedes usar cualquier recurso para el desarrollo, pero debes poder defender el código.
* Para ser aprobado, el sistema debe funcionar correctamente y cumplir con todos los requisitos.
* El trabajo debe ser defendido explicando oralmente qué realiza cada línea de código.
* Dispone de Bootstrap para el diseño de la interfaz, pero no es obligatorio. 
* Se entregan dos proyectos `servidor` y `cliente` para que tenga un punto de entrada. El código de ambos solo tiene el esqueleto del proyecto, no contiene ninguna funcionalidad y debe ser modificado por completo para desarrollar la solución. 
* Para probar el sistema debe ir a la carpeta del servidor y ejecutar el comando `dotnet run` para iniciar el servidor. Luego, en otra terminal, ir a la carpeta del cliente y ejecutar `dotnet run` para iniciar el cliente.

> **Consejo**: comienza por el modelo de datos y pruebas unitarias del API; luego integra el cliente gradualmente. Versiona tu progreso con commits descriptivos: te será más fácil depurar y justificar tus decisiones en la defensa. ¡Éxitos!
