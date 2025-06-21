# Scripts para Mergear Pull Requests Automáticamente

Este conjunto de scripts te permite mergear todos los pull requests abiertos de tu repositorio, sobrescribiendo conflictos según sea necesario.

## 📋 Requisitos Previos

1. **GitHub CLI instalado y autenticado:**
   ```bash
   brew install gh
   gh auth login
   ```

2. **jq instalado (para procesar JSON):**
   ```bash
   brew install jq
   ```

3. **Estar en el directorio del repositorio git**

## 🔧 Scripts Disponibles

### 1. `merge_all_prs.sh` - Script Inteligente (Recomendado)
Este es el script principal que mergea todos los PRs usando estrategias inteligentes de resolución de conflictos.

**Uso:**
```bash
./merge_all_prs.sh
```

**Características:**
- ✅ Mergea PRs con estrategia `theirs` (sobrescribe conflictos)
- ✅ Crea ramas temporales para evitar problemas
- ✅ Cierra automáticamente los PRs
- ✅ Manejo de errores robusto
- ✅ Output colorizado y informativo

### 2. `merge_all_prs_aggressive.sh` - Script Agresivo
Script más agresivo que literalmente sobrescribe el contenido de main con cada PR.

**Uso:**
```bash
./merge_all_prs_aggressive.sh
```

**⚠️ ADVERTENCIA:**
- 🔥 **EXTREMADAMENTE DESTRUCTIVO**
- 🔥 Sobrescribe completamente el contenido de main
- 🔥 Solo usar si quieres que main tenga exactamente el contenido del último PR procesado

### 3. `merge_prs_interactive.sh` - Script Interactivo (Más Control)
Script que te permite elegir qué hacer con cada PR individualmente.

**Uso básico:**
```bash
./merge_prs_interactive.sh
```

**Uso automático:**
```bash
./merge_prs_interactive.sh --auto          # Mergea todos automáticamente
./merge_prs_interactive.sh --auto --force  # Mergea todos con sobrescritura total
```

**Opciones por PR:**
1. **Merge normal** - Merge tradicional de git
2. **Merge con sobrescritura** - Resuelve conflictos automáticamente
3. **Sobrescritura total** - Reemplaza main completamente
4. **Saltar PR** - No procesa este PR
5. **Salir** - Termina el script

## 🛡️ Seguridad

Todos los scripts crean automáticamente un **backup de la rama main** antes de hacer cambios:
- Formato: `backup-main-YYYYMMDD-HHMMSS`
- Se pushea automáticamente al remoto
- Puedes restaurar con: `git checkout backup-main-YYYYMMDD-HHMMSS`

## 📖 Ejemplo de Uso

```bash
# 1. Navegar al repositorio
cd /Users/adibattista/Documents/GitHub/tup-25-p3

# 2. Verificar que GitHub CLI está autenticado
gh auth status

# 3. Ejecutar el script recomendado
./merge_all_prs.sh

# O usar el interactivo para más control
./merge_prs_interactive.sh
```

## 🔍 Lo que hacen los scripts:

1. **Verifican** que estés en un repo git y GitHub CLI esté autenticado
2. **Crean backup** de la rama main actual
3. **Obtienen** lista de todos los PRs abiertos
4. **Procesan** cada PR:
   - Fetch de los cambios
   - Merge con la estrategia especificada
   - Push de los cambios
   - Cierre automático del PR
5. **Limpian** ramas temporales
6. **Reportan** el resumen de operaciones

## ⚠️ Consideraciones Importantes

- 🔄 **Los conflictos se resuelven automáticamente** sobrescribiendo con el contenido del PR
- 📝 **Los PRs se cierran automáticamente** con un comentario explicativo
- 💾 **Siempre se crea un backup** de main antes de empezar
- 🔍 **Revisa el estado final** del repositorio después de ejecutar

## 🆘 Si algo sale mal

1. **Restaurar desde backup:**
   ```bash
   git checkout backup-main-YYYYMMDD-HHMMSS
   git checkout -b main-restored
   git push origin main-restored
   ```

2. **Ver historial de operaciones:**
   ```bash
   git log --oneline -10
   ```

3. **Revertir último commit:**
   ```bash
   git revert HEAD
   ```

## 📧 Soporte

Si tienes problemas, verifica:
- Que GitHub CLI esté autenticado: `gh auth status`
- Que tengas permisos de push al repositorio
- Que no haya operaciones git en progreso: `git status`
