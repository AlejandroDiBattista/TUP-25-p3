# 🛒 ElectroShop - Tienda Online

## 📋 Descripción
ElectroShop es una aplicación web de ejemplo para la gestión de una tienda online, desarrollada como trabajo práctico para la materia Programación 3. Incluye frontend en Blazor WebAssembly, backend con Minimal API en C#, y persistencia con Entity Framework Core y SQLite.

---

## ✨ Características principales

### 💻 Frontend (Blazor WebAssembly)
- 🗂️ **Catálogo de productos**: listado, búsqueda, tarjetas con imagen, nombre, descripción, stock y precio.
- 📌 **Navbar fija**: logo, nombre de la tienda, enlaces a Catálogo y Carrito, ícono de carrito con contador.
- 🛒 **Carrito de compras**: listado de productos, controles +/- para modificar cantidad, vaciar carrito, confirmar compra.
- ✅ **Confirmación de compra**: resumen, formulario de datos del cliente (nombre, apellido, email), validaciones.
- 🎉 **Pop-up de confirmación**: mensaje visual al confirmar la compra.

### 🖥️ Backend (Minimal API C#)
- **Endpoints REST**:
  - 🔍 `GET /productos` (con búsqueda por query)
  - 🆕 `POST /carritos` (nuevo carrito)
  - 📦 `GET /carritos/{carritoId}` (ver ítems)
  - 🗑️ `DELETE /carritos/{carritoId}` (vaciar carrito)
  - ➕ `PUT /carritos/{carritoId}/{productoId}` (agregar/actualizar producto)
  - ➖ `DELETE /carritos/{carritoId}/{productoId}` (eliminar producto)
  - ✔️ `POST /carritos/{carritoId}/confirmar` (confirmar compra, registrar, descontar stock y limpiar carrito)
- 🛡️ **Validaciones**: stock, datos del cliente, persistencia de cambios.

### 💾 Persistencia (EF Core + SQLite)
- **Modelos**: Productos, Compras, Items de compra, Carritos, Items de carrito.
- **Carga inicial**: 10 productos de ejemplo con imágenes y datos realistas.

---

## 🗂️ Estructura del proyecto

- `📁 cliente/` - Frontend Blazor WebAssembly
  - `📄 Pages/` - Páginas principales (Home, Carrito, etc.)
  - `🔌 Services/` - Servicios para consumir la API
  - `🖼️ wwwroot/` - Archivos estáticos (imágenes, favicon, logo, CSS)
- `📁 servidor/` - Backend Minimal API
  - `⚙️ Program.cs` - Configuración de endpoints y seed de productos
  - `📦 Models/` - Modelos de datos
  - `🗃️ TiendaContext.cs` - Contexto EF Core

---

## 🚀 Cómo ejecutar el proyecto

1. 📥 **Clonar el repositorio** y abrir la solución en Visual Studio o VS Code.
2. 🛠️ **Restaurar dependencias** (dotnet restore).
3. ▶️ **Ejecutar el backend** (servidor):
   - Navegar a la carpeta `servidor/` y ejecutar `dotnet run`.
4. ▶️ **Ejecutar el frontend** (cliente):
   - Navegar a la carpeta `cliente/` y ejecutar `dotnet run` o usar el comando de Blazor correspondiente.
5. 🌐 Acceder a la aplicación desde el navegador en la URL indicada (por defecto http://localhost:5177 o similar).

---

## 👨‍💻 Créditos
- Autor: Mariano Moya
- Año: 2025
- Materia: Programación 3

---

