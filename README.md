# TP5

> **Fecha de entrega**: Viernes 23/05/2025 hasta las 23:59

## Consigna 

Realizar una aplicación cliente/servidor que administre el stock de una tienda de productos.

### Aplicación cliente

Implementar una aplicación de consola que:

- Liste los productos disponibles en la tienda.
- Liste los productos que se deben reponer (stock menor a 3 unidades).
- Permita agregar stock a un producto.
- Permita quitar stock a un producto.

La aplicación se debe conectar al servidor REST que implemente las funciones correspondientes a cada una de las acciones usando `HttpClient`.

### Aplicación servidor

El servidor debe implementar una API REST que permita:
- Listar los productos disponibles en la tienda.
- Listar los productos que se deben reponer (stock menor a 3 unidades).
- Agregar stock a un producto.
- Quitar stock a un producto (debe validar que el stock no quede negativo).

Los datos deben ser persistidos en SQLite usando Entity Framework Core.
El servidor se debe implementar usando Minimal API.

Al comenzar, crear 10 ejemplos de productos con un stock inicial de 10 unidades cada uno.

> NOTA: Los programas están hechos para que se ejecuten con `dotnet script`.