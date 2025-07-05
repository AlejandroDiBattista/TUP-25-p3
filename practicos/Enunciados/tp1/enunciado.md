#TP1 : Agenda de Contactos

Diseña un programa de consola que administre una **agenda de contactos** cumpliendo los siguientes requisitos:

1. **Identificación por ID**  
   - Cada contacto tendrá un **ID único** (entero).  
   - El programa debe asignar automáticamente el ID de forma incremental a cada contacto nuevo.

2. **Estructura de Almacenamiento**  
   - Debes utilizar un **`struct`** (por ejemplo, `struct Contacto`) con los campos `Id`, `Nombre`, `Telefono` y `Email`.  
   - Se recomienda usar un **array** para guardar los contactos.  
   - No se permite el uso de clases personalizadas, LINQ, `foreach` ni enumeradores. Solo **struct**, **arrays**, `if` y `for`.

3. **Funciones Principales**  
   - **Agregar contacto**: Se pide al usuario el nombre, teléfono y email. El programa asigna un nuevo ID y registra el contacto en el array.  
   - **Modificar contacto**: Se solicita el **ID** del contacto a cambiar. Luego, se permiten actualizar individualmente (o dejar sin cambios) el nombre, teléfono y email.  
   - **Borrar contacto**: El usuario ingresa el ID del contacto para eliminarlo de la agenda (ajusta el array según sea necesario).  
   - **Listar contactos**: Muestra todos los contactos en **formato tabular**, con columnas alineadas para ID, nombre, teléfono y email.  
   - **Buscar contacto**: Pide un **término de búsqueda** y debe mostrar solo aquellos contactos cuyo nombre, teléfono o email contengan dicho término (ignorando mayúsculas/minúsculas).  

4. **Interfaz de Usuario**  
   - El programa deberá funcionar mediante un menú textual en la consola:  
     1. Agregar contacto  
     2. Modificar contacto  
     3. Borrar contacto  
     4. Listar contactos  
     5. Buscar contacto  
     6. Salir  

5. **Manejo de Archivos**  
   - Al iniciar la aplicación, se deben leer los contactos almacenados en el archivo **`agenda.csv`** (si existe) y cargarlos en el array.  
   - Antes de que finalice la ejecución (opción “Salir”), el programa debe guardar todos los contactos de vuelta en **`agenda.csv`** para que los cambios persistan.  

6. **Consideraciones Especiales**  
   - El listado de contactos debe mostrarse usando un **formato de columnas alineadas** para mejorar la lectura (por ejemplo, especificadores de formato en `Console.WriteLine`).  
   - Para recorrer el array y acceder a cada contacto, utiliza únicamente bucles `for`.  
   - El programa debe validar que no se agreguen contactos más allá del límite de tu array.  
   - Al finalizar, se debe poder **agregar, modificar, borrar, listar y buscar** contactos de manera adecuada.  
   - El programa continúa ofreciendo el menú en un bucle hasta que el usuario elija la opción “Salir”.


**Objetivo**  
Consolidar el uso de:  
- `struct` en C#  
- Manejo de datos utilizando solamente `if`, `for` y asignaciones en arrays  
- Generación de un menú en consola  
- Formateo de salida para mostrar tablas  
- Lectura y escritura de datos mediante un archivo CSV  

**Entrega**  
- Proporcionar el archivo .cs con todo el código de la aplicación.  
- El programa debe poder compilarse y ejecutarse correctamente en la consola de C#.

**Ejemplo de Ejecución**  
```bash
===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 1

=== Agregar Contacto ===
Nombre   : Juan Pérez
Teléfono : 123456
Email    : juan@example.com
Contacto agregado con ID = 1
Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 1

=== Agregar Contacto ===
Nombre   : María López
Teléfono : 654321
Email    : mlopez@mail.com
Contacto agregado con ID = 2
Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 4

=== Lista de Contactos ===
ID    NOMBRE               TELÉFONO       EMAIL                    
1     Juan Pérez           123456         juan@example.com         
2     María López          654321         mlopez@mail.com          

Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 5

=== Buscar Contacto ===
Ingrese un término de búsqueda (nombre, teléfono o email): juan

Resultados de la búsqueda:
ID    NOMBRE               TELÉFONO       EMAIL                    
1     Juan Pérez           123456         juan@example.com         

Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 2

=== Modificar Contacto ===
Ingrese el ID del contacto a modificar: 1
Datos actuales => Nombre: Juan Pérez, Teléfono : 123456, Email: juan@example.com
(Deje el campo en blanco para no modificar)

Nombre    : Juan P. Domínguez
Teléfono  : 
Email     : 

Contacto modificado con éxito. 
Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 4

=== Lista de Contactos ===
ID    NOMBRE               TELÉFONO       EMAIL                    
1     Juan P. Domínguez    123456         juan@example.com         
2     María López          654321         mlopez@mail.com          

Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 3

=== Borrar Contacto ===
Ingrese el ID del contacto a borrar: 2
Contacto con ID=2 eliminado con éxito.
Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 4

=== Lista de Contactos ===
ID    NOMBRE               TELÉFONO       EMAIL                    
1     Juan P. Domínguez    123456         juan@example.com         

Presione cualquier tecla para continuar...

===== AGENDA DE CONTACTOS =====
1) Agregar contacto
2) Modificar contacto
3) Borrar contacto
4) Listar contactos
5) Buscar contacto
0) Salir
Seleccione una opción: 6

Saliendo de la aplicación...
````