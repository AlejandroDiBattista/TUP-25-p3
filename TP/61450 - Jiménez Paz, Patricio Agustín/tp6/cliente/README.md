# Instrucciones de uso y compilación

## Requisitos
- .NET 6 o superior
- Node.js (solo si vas a modificar clases de Tailwind)

## Compilación y ejecución
1. Abre una terminal en la carpeta del proyecto.
2. Ejecuta:
   ```sh
   dotnet build
   dotnet run
   ```

## Modificar clases de Tailwind
1. Asegúrate de tener Node.js instalado.
2. Instala las dependencias:
   ```sh
   npm install
   ```
3. Descomenta las líneas correspondientes a TailwindCSS en el archivo .csproj **o, alternativamente**, puedes compilar Tailwind manualmente ejecutando:
  ```sh
  npx @tailwindcss/cli -i './wwwroot/css/tailwind.css' -o './wwwroot/css/app.css'
  ```
4. Realiza los cambios en los archivos .razor o .html con las clases de Tailwind.
5. Compila y ejecuta normalmente.
