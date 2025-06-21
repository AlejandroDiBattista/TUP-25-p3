#!/bin/bash

# Script para mergear todos los pull requests abiertos sobrescribiendo conflictos
# Autor: Script generado para mergear PRs forzosamente
# Fecha: $(date)

set -e  # Salir si hay algún error

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== SCRIPT PARA MERGEAR TODOS LOS PULL REQUESTS ===${NC}"
echo -e "${YELLOW}⚠️  ADVERTENCIA: Este script va a sobrescribir archivos sin preguntar ⚠️${NC}"
echo ""

# Verificar que estamos en un repositorio git
if [ ! -d ".git" ]; then
    echo -e "${RED}Error: No estás en un repositorio git${NC}"
    exit 1
fi

# Verificar que GitHub CLI está autenticado
if ! gh auth status &>/dev/null; then
    echo -e "${RED}Error: GitHub CLI no está autenticado. Ejecuta 'gh auth login'${NC}"
    exit 1
fi

# Cambiar a la rama main
echo -e "${BLUE}📋 Cambiando a la rama main...${NC}"
git checkout main
git pull origin main

# Obtener todos los pull requests abiertos
echo -e "${BLUE}🔍 Obteniendo lista de pull requests abiertos...${NC}"
prs=$(gh pr list --state open --json number,title,headRefName,author --limit 100)

if [ "$prs" = "[]" ]; then
    echo -e "${GREEN}✅ No hay pull requests abiertos${NC}"
    exit 0
fi

# Contar PRs
pr_count=$(echo "$prs" | jq length)
echo -e "${YELLOW}📊 Se encontraron $pr_count pull request(s) abierto(s)${NC}"
echo ""

# Procesar cada PR
echo "$prs" | jq -r '.[] | "\(.number)|\(.title)|\(.headRefName)|\(.author.login)"' | while IFS='|' read -r pr_number pr_title head_ref author; do
    echo -e "${BLUE}🔄 Procesando PR #$pr_number: '$pr_title' por @$author${NC}"
    echo -e "${YELLOW}   Rama: $head_ref${NC}"
    
    # Crear una rama temporal para el merge
    temp_branch="temp-merge-pr-$pr_number"
    
    # Limpiar cualquier rama temporal anterior
    git branch -D "$temp_branch" 2>/dev/null || true
    
    # Crear rama temporal desde main
    git checkout -b "$temp_branch" main
    
    # Fetch de la rama del PR
    echo -e "${YELLOW}   📥 Descargando cambios del PR...${NC}"
    git fetch origin "refs/pull/$pr_number/head:pr-$pr_number" 2>/dev/null || {
        echo -e "${RED}   ❌ Error al obtener el PR #$pr_number${NC}"
        git checkout main
        git branch -D "$temp_branch" 2>/dev/null || true
        continue
    }
    
    # Mergear forzosamente (estrategia theirs para sobrescribir conflictos)
    echo -e "${YELLOW}   🔀 Mergeando con estrategia de sobrescritura...${NC}"
    if git merge "pr-$pr_number" -X theirs --no-edit; then
        echo -e "${GREEN}   ✅ Merge exitoso${NC}"
        
        # Volver a main y mergear los cambios
        git checkout main
        git merge "$temp_branch" --no-edit
        
        # Pushear cambios a main
        echo -e "${YELLOW}   📤 Pusheando cambios a main...${NC}"
        git push origin main
        
        # Cerrar el pull request
        echo -e "${YELLOW}   🔒 Cerrando pull request...${NC}"
        gh pr close "$pr_number" --comment "✅ Mergeado automáticamente via script - contenido sobrescrito en main"
        
        echo -e "${GREEN}   ✅ PR #$pr_number cerrado exitosamente${NC}"
        
    else
        echo -e "${RED}   ❌ Error en el merge del PR #$pr_number${NC}"
        # En caso de conflictos irresolubles, forzar con reset
        echo -e "${YELLOW}   🔧 Intentando merge forzado con reset...${NC}"
        
        git reset --hard "pr-$pr_number"
        git checkout main
        git reset --hard "$temp_branch"
        git push --force origin main
        
        # Cerrar el pull request
        gh pr close "$pr_number" --comment "⚠️ Mergeado forzosamente via script - contenido completamente sobrescrito"
        echo -e "${YELLOW}   ⚠️ PR #$pr_number mergeado forzosamente${NC}"
    fi
    
    # Limpiar ramas temporales
    git branch -D "$temp_branch" 2>/dev/null || true
    git branch -D "pr-$pr_number" 2>/dev/null || true
    
    echo ""
done

echo -e "${GREEN}🎉 ¡Proceso completado! Todos los PRs han sido procesados.${NC}"
echo -e "${BLUE}📋 Resumen: Se procesaron $pr_count pull request(s)${NC}"
echo -e "${YELLOW}⚠️  Recuerda revisar el estado final del repositorio${NC}"
