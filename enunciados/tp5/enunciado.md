# TP5

> **Fecha de entrega**: Viernes 23/05/2025 hasta las 23:59

## Consigna 

Realizar una aplicacion cliente/servidos que administre el stock de una tienda de productos.

### Aplicacion cliente

Implementar una aplicacion de consola que:

- Liste los productos disponibles en la tienda.
- Liste los productos que se deben reponer (stock menor a 3 unidades).
- Permita agregar stock a un producto.
- Permita quitar stock a un producto.

La aplicacion se debe conectar al servidor rest que implemente las funciones correspondientes a cada una de las acciones usando HttpClient.

### Aplicacion servidor

El servidor debe implementar un API REST que permita:
- Listar los productos disponibles en la tienda.
- Listar los productos que se deben reponer (stock menor a 3 unidades).
- Agregar stock a un producto.
- Quitar stock a un producto (Debe validar que el stock no quede negativo).

Los datos deben ser persistidos en SQLite usando Entity Framework Core.
El servidor se debe implementar usando Minimal API.

Al comenzar crear crear 10 ejemplos de productos con un stock inicial de 10 unidades cada uno.

> NOTA: Los programas estan hecho para que se ejecute con `dotnet script` 