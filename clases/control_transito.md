# Proyecto: Sistema de Control de Tránsito

## Descripción General

El objetivo es desarrollar un sistema interactivo que permita monitorear y registrar la cantidad y el tipo de vehículos que transitan por un punto específico durante un intervalo de tiempo determinado.

La aplicación web deberá ser compatible con dispositivos móviles y computadoras de escritorio, ofreciendo las siguientes funcionalidades principales:

1.  **Configuración de Contadores:** Permitir al usuario definir dinámicamente la cantidad de contadores necesarios, asignando un nombre descriptivo a cada uno (por ejemplo, "Autos", "Motos", "Camiones").
2.  **Proceso de Conteo:**
    *   Implementar una cuenta regresiva de 10 minutos.
    *   Mostrar cada contador en formato de lista o tarjetas individuales.
    *   Cada elemento del contador deberá visualizar:
        *   El nombre asignado.
        *   La cantidad actual de vehículos registrados para ese tipo.
        *   Un botón o área interactiva que, al ser pulsada, incremente en uno el valor del contador correspondiente.
3.  **Visualización de Resultados:**
    *   Al concluir el período de 10 minutos (o al accionar un botón de finalización manual), el sistema deberá presentar:
        *   Una tabla resumen con los datos detallados: nombre del tipo de vehículo y cantidad total y cantidad de vehículos por hora.

---

## Especificaciones Técnicas y de Diseño

### Plataforma y Estructura
*   **Tecnología Principal:** Blazor WebAssembly (ejecución del lado del cliente).
*   **Arquitectura:** Aplicación de Página Única (SPA - Single Page Application) para una experiencia de usuario fluida y sin recargas de página completas.
*   **Estilos:** CSS puro, sin recurrir a frameworks como Bootstrap. Se busca un diseño personalizado y moderno.

### Interfaz de Usuario (UI) y Experiencia de Usuario (UX)

*   **Estética General:** Moderna, limpia y atractiva.
    1.  **Formularios:** Deben estar dentro de un contenedor que simule una tarjeta, con un título en la parte superior y botones de aceptar y cancelar alineados a la derecha en la parte inferior.
    2.  **Etiquetas:** Deben estar ubicadas arriba de los campos de texto, alineada a la izquierda y con una tipografía legible.
    3.  **Campos de Entrada:** Esquinas redondeadas (ej. `border-radius: 10px;`) y un ligero efecto de "hundido" o `inset`.
    4.  **Botones:** Bordes redondeados y un sutil efecto de relieve (simulando elevación o `box-shadow`), fondo blanco semitransparente.
    5.  **Paleta de Colores:** Fondos con degradados suaves utilizando colores pasteles.
*   **Flujo de la Aplicación:**
    1.  **Pantalla Inicial:** Solicitar al usuario la cantidad de tipos de vehículos que desea monitorear.
    2.  **Pantalla de Nomenclatura:** Presentar campos de entrada para que el usuario asigne un nombre a cada uno de los contadores definidos en el paso anterior.
    3.  **Pantalla de Conteo Activo:**
        *   Visualizar los contadores en formato de tarjetas. Cada tarjeta debe:
            *   Mostrar el nombre del contador como título.
            *   Presentar la cuenta actual de forma prominente en el centro (vertical y horizontalmente), con un tamaño de fuente legible (ej. `24px`).
            *   Ser completamente interactiva: toda la tarjeta funcionará como un botón para incrementar el contador asociado.
        *   En la parte superior de esta pantalla, un temporizador visible mostrará la cuenta regresiva de los 10 minutos.
        *   Un botón "Terminar" (ubicado preferentemente en la parte inferior) permitirá concluir el conteo antes de que finalice el tiempo.
        *   **Efecto Visual:** Al incrementar un contador, el número correspondiente deberá tener una breve animación (ej. agrandarse ligeramente y volver a su tamaño original) para dar retroalimentación visual.
    4.  **Pantalla de Resultados:**
        *   Mostrar el gráfico de torta en la sección superior.
        *   Debajo del gráfico, una tabla detallará los nombres y las cantidades finales de cada contador.
    5.  **Reinicio:** Incluir un botón "Comenzar Nuevamente" para permitir al usuario iniciar un nuevo ciclo de conteo desde la pantalla inicial.

---

## Consideraciones Adicionales

*   **Propósito Educativo:** Este proyecto está concebido como una herramienta de enseñanza para introducir a los estudiantes en la programación con Blazor. Por lo tanto, la simplicidad, claridad del código y la documentación interna son prioritarias sobre la robustez o características avanzadas de un sistema de producción.
*   **Autonomía:** La aplicación debe ser autocontenida, sin dependencias de bases de datos externas o servicios de backend. Toda la lógica y el almacenamiento temporal de datos residirán en el cliente.
*   **Simplificaciones:** No es necesario implementar mecanismos de seguridad complejos ni validaciones de datos exhaustivas. El enfoque principal es el aprendizaje de los conceptos fundamentales de Blazor.
*   **Entorno de Desarrollo:**
    *   Se asume que .NET SDK está instalado en el entorno de desarrollo.
    *   El proyecto deberá crearse desde cero en una carpeta denominada `D3`, ubicada dentro de un directorio `clases`.
*   **Gestión de Datos del Contador:** Para una mejor organización y mantenibilidad del código, se recomienda encapsular la información de cada contador (nombre, valor actual, y cualquier estado relevante como el de animación) dentro de una clase específica en C#.

*   **Consideraciones extras**
    * Eliminar toda funcionalidad innecesaria entre ella los archivos de estilos y los ejemplos de  uso que tiene la plantilla de blazor.
*   **Documentación:** Incluir comentarios claros y concisos en el código, explicando la funcionalidad de cada sección y los métodos utilizados.
---
