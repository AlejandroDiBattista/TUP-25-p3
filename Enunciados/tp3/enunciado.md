# TP3: Crear Lista Ordenada Genérica

## Objetivo
Aprender a desarrollar estructuras de datos reutilizables.

## Tarea

Se debe crear una clase `ListaOrdenada` que permita agregar un elemento, verificar si contiene un elemento y eliminar un elemento.  

Además, se deben poder leer los elementos según su posición ([]) y determinar cuántos elementos componen la lista.  

Por último, se debe poder filtrar todos los elementos que cumplan una condición.

Funciones:
- `Contiene(elemento)` : Indica si el elemento existe en la lista
- `Agregar(elemento)` : Agrega un elemento manteniendo la lista ordenada (si esta repetido ignorar)
- `Eliminar(elemento)` : Elimina un elmento de la lista (si no existe ignorar)
- `Cantidad`: Indica la cantidad de elementos que hay en la lista.
- `Lista[indice]`: Me retornar el elemento que esta en la posicion indicada por el indice.
- `Filtrar(condicion)` : Me da una nueva lista ordenada con todos los elemento que cumpla la condicion.

Ademas debe crear la clase `Contacto` con `Nombre` y `Telefono` que debe mantenerse ordenanda alfabeticamente.

## Presentación

El trabajo consiste en implementar las clases `ListaOrdenada` y `Contacto` para que se ejecuten todas las pruebas sin errores.  

No se debe crear ninguna interfaz de usuario; únicamente se deberán pasar los test exitosamente.

Solo debe modificar el archivo `ejercicio.cs` de la carpeta `tp3` y solo debe subir dicho archivo. 

## Plazo de entrega

> Viernes 11 de abril hasta las 23:59 hs
