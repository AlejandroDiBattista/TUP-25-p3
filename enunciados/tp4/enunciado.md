# TP4: Sistema de Exámenes Multiple Choice en Consola usando Entity Framework Core

> **Fecha de entrega:** Viernes 09/05/2025 23:59

## Objetivo General

El objetivo del trabajo práctico es diseñar y desarrollar una aplicación de consola en C# que permita:
- Registrar preguntas de opción múltiple.
- Tomar exámenes aleatorios.
- Evaluar al alumno.
- Registrar los resultados históricos.
- Generar reportes estadísticos básicos.

El sistema deberá usar Entity Framework Core para persistir todos los datos en una base de datos SQLite.

⸻

## Requisitos Funcionales
1.	**Registro de Preguntas**
- Cada pregunta debe tener:
    * Enunciado de la pregunta.
    * Tres alternativas de respuesta (A, B, C).
    * Indicación de cuál es la respuesta correcta.

2.	**Toma de Examen**
* El sistema debe permitir al alumno ingresar su nombre.
* El sistema debe seleccionar cinco preguntas aleatorias de la base de datos.
* El alumno debe responder a cada pregunta seleccionando A, B o C.
- Se debe calcular:
    * La cantidad de respuestas correctas.
    * La nota final sobre 5 puntos.

3.	**Registro de Resultados**
-	Se debe almacenar:
    * Nombre del alumno.
    * Cantidad de respuestas correctas.
    * Total de preguntas.
    * Nota final.

4.	**Registro de Respuestas**
-	Para cada pregunta respondida, debe almacenarse:
    * A qué examen pertenece.
    * Qué pregunta fue.
    * Si fue respondida correctamente o no.

5.	**Reportes**
-	Mostrar listado de todos los exámenes rendidos.
-	Filtrar resultados por nombre de alumno.
-	Mostrar un ranking de los mejores alumnos basado en la mejor nota obtenida.
-	Mostrar un informe estadístico por pregunta, que incluya:
    * Cuántas veces fue respondida.
    * Qué porcentaje de respuestas fueron correctas.

---

**Requisitos Técnicos**
* La persistencia de datos debe ser realizada usando Entity Framework Core con base de datos SQLite.
* El sistema debe estar implementado como aplicación de consola (no GUI).
* Debe respetar una estructura limpia de clases:
    * Pregunta
    * ResultadoExamen
    * RespuestaExamen
* Debe usar correctamente los conceptos de:
    * DbContext.
    * Relaciones entre tablas (1 a muchos).
    * Consultas con LINQ.

---

**Notas adicionales**
* El número de preguntas del examen debe ser 5 por defecto.
* Si hay menos de 5 preguntas disponibles, el sistema debe realizar el examen con las preguntas que haya.
* No se permite el uso de frameworks gráficos ni web.
* El proyecto debe ser entregado en un único directorio, con el código fuente completo.

