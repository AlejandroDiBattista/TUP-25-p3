# Aplicación de Administración de Stock

Esta aplicación permite administrar el stock de una tienda de productos.

## Estructura del Proyecto

- `Server/` - Servidor REST con Minimal API y Entity Framework Core
- `Client/` - Aplicación cliente de consola

## Cómo ejecutar

### 1. Ejecutar el Servidor

```bash
cd Server
dotnet run
```

El servidor se ejecutará en `http://localhost:5000`

### 2. Ejecutar el Cliente

En otra terminal:

```bash
cd Client
dotnet run
```

## Funcionalidades

### Cliente
- Listar productos disponibles
- Listar productos con stock bajo (menos de 3 unidades)
- Agregar stock a un producto
- Quitar stock a un producto

### Servidor
- API REST con todos los endpoints necesarios
- Base de datos SQLite con Entity Framework Core
- 10 productos de ejemplo con stock inicial de 10 unidades cada uno
- Validación para evitar stock negativo

## Nota
Asegúrate de tener .NET 7.0 instalado en tu sistema. 