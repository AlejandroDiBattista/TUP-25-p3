# Como subir las correcciones.

Para subir las correcciones primero deben cambiar a la rama `main`
Luego traer los cambios `fech origin main`
Luego deben crear una nueva rama 
Deben realizar los cambios en la nueva rama
Confirmar los cambios
Publicarlos
Y crear el pull request.

```bash
git checkout main
git pull origin main
git checkout -b nueva-rama
# Realizar cambios
git add .
git commit -m "Descripción de los cambios"
git push origin nueva-rama
```