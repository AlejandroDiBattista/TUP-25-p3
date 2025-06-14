# Tienda Online - Blazor WASM & .NET Minimal API

Este proyecto es una aplicación web completa de comercio electrónico desarrollada como parte de la evaluación de la materia. La solución demuestra el dominio de tecnologías modernas del ecosistema .NET.

---

## 1. Objetivo

Modificar los archivos base de la carpeta "C:\Users\54381\OneDrive\Escritorio\TUP-25-p3\TP\61271 - Donelli, Gerardo Exequiel\tp6", no trabajar fuera de esta ruta una aplicación web completa (“Tienda Online”) que demuestre dominio de:
-   **Frontend:** Blazor WebAssembly para una experiencia de usuario rica e interactiva.
-   **Backend:** Minimal API en C# que exponga un servicio RESTful.
-   **Persistencia:** Entity Framework Core con una base de datos SQLite.

---

## 2. Requisitos Funcionales

### 2.1. Frontend – Blazor WASM

-   **Catálogo de productos:**
    -   Listado de productos disponible y buscable.
    -   Cabecera fija con logo (home), buscador e ícono de carrito con contador de ítems.
    -   Tarjetas de producto con imagen, nombre, descripción, stock disponible y precio.
    -   Botón `Agregar al carrito` que valida el stock antes de añadir un producto.

-   **Carrito de compra:**
    -   Lista detallada de productos, unidades, precio unitario e importe subtotal.
    -   Controles `+` y `-` para modificar la cantidad de cada ítem, ajustando el stock.
    -   Botones `Vaciar carrito` y `Confirmar compra`.

-   **Confirmación de compra:**
    -   Resumen del pedido con el total de ítems y el importe final.
    -   Formulario para datos del cliente con campos obligatorios: Nombre, Apellido y Email.
    -   Botón `Confirmar` que registra la compra en el sistema, limpia el carrito y redirige al catálogo.

### 2.2. Backend – Minimal API

Funciones expuestas por la API REST:

| Verbo | Ruta                                      | Descripción                                      |
| :---- | :---------------------------------------- | :----------------------------------------------- |
| `GET` | `/api/productos`                          | Obtiene productos (permite filtrar por nombre).  |
| `POST`| `/api/compras`                            | Registra una nueva compra (recibe datos de cliente y carrito). |


---

## 3. Modelo de Datos

-   **`Productos`**: `Id`, `Nombre`, `Descripción`, `Precio`, `Stock`, `ImagenUrl`.
-   **`Compras`**: `Id`, `Fecha`, `Total`, `NombreCliente`, `ApellidoCliente`, `EmailCliente`.
-   **`Items de compra`**: `Id`, `ProductoId`, `CompraId`, `Cantidad`, `PrecioUnitario`.

> **Datos iniciales**: El sistema se inicializa con al menos 10 productos de ejemplo consistentes (ej: tecnología) y con imágenes representativas.

---

## 4. Plan de Commits (Checklist de Progreso)

-   [ ] **Commit 1:** Creación de la solución y estructura de los proyectos (Servidor y Cliente).
-   [ ] **Commit 2:** Definición de los modelos de datos (Producto, Compra, ItemCompra).
-   [ ] **Commit 3:** Configuración de Entity Framework Core con SQLite y creación del `DbContext`.
-   [ ] **Commit 4:** Implementación del "seeding" para cargar los 10 productos iniciales.
-   [ ] **Commit 5:** Creación de la migración inicial y aplicación a la base de datos.
-   [ ] **Commit 6:** Desarrollo del endpoint `GET /api/productos` con lógica de búsqueda.
-   [ ] **Commit 7:** Desarrollo del endpoint `POST /api/compras` con validación de stock y transacciones.
-   [ ] **Commit 8:** Configuración del Frontend (servicios para consumir la API y estado del carrito).
-   [ ] **Commit 9:** Creación de la página del Catálogo de Productos y la tarjeta de producto.
-   [ ] **Commit 10:** Implementación de la página del Carrito de Compras con su funcionalidad.
-   [ ] **Commit 11:** Creación del formulario y página de Confirmación de Compra.
-   [ ] **Commit 12:** Diseño del Layout principal, cabecera y contador del carrito.
-   [ ] **Commit 13:** Pruebas E2E, ajustes finales y limpieza de código.

---

## 5. Cómo Ejecutar el Proyecto

Para poder probar el sistema, sigue estos pasos:

1.  **Iniciar el Servidor (Backend):**
    -   Abre una terminal en la carpeta del proyecto del **servidor**.
    -   Ejecuta el comando:
        ```sh
        dotnet run
        ```

2.  **Iniciar el Cliente (Frontend):**
    -   Abre **otra** terminal en la carpeta del proyecto del **cliente**.
    -   Ejecuta el comando:
        ```sh
        dotnet run
        ```
    -   Abre tu navegador y ve a la dirección que indica la terminal del cliente (usualmente `http://localhost:XXXX`).

---

## 6. Notas de Desarrollo

-   **Simplicidad:** El código debe ser claro y estar bien documentado para facilitar la defensa oral.
-   **Consistencia:** Los productos de ejemplo deben ser temáticos (ej: celulares, accesorios, etc.).
-   **Validaciones:** La validación de stock debe ser robusta y realizarse en tiempo real.
-   **Bootstrap:** El uso de Bootstrap está disponible para el diseño de la interfaz, pero no es obligatorio.
-   **Imágenes:** Se deben utilizar URLs representativas para las imágenes de los productos.
