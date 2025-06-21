#!/bin/bash

# Script alternativo más agresivo para mergear PRs sobrescribiendo TODO
# Este script literalmente reemplaza el contenido de main con el contenido de cada PR

set -e

# Colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}=== SCRIPT AGRESIVO PARA MERGEAR TODOS LOS PRs ===${NC}"
echo -e "${RED}⚠️⚠️⚠️  EXTREMA PRECAUCIÓN: SOBRESCRIBIRÁ TODO EL CONTENIDO  ⚠️⚠️⚠️${NC}"
echo ""

read -p "¿Estás ABSOLUTAMENTE seguro? Esto sobrescribirá main completamente (y/N): " confirm
if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
    echo "Operación cancelada"
    exit 0
fi

# Verificaciones
if [ ! -d ".git" ]; then
    echo -e "${RED}Error: No estás en un repositorio git${NC}"
    exit 1
fi

if ! gh auth status &>/dev/null; then
    echo -e "${RED}Error: GitHub CLI no está autenticado${NC}"
    exit 1
fi

# Backup de main
echo -e "${BLUE}💾 Creando backup de main...${NC}"
git checkout main
git pull origin main
backup_branch="backup-main-$(date +%Y%m%d-%H%M%S)"
git checkout -b "$backup_branch"
git push origin "$backup_branch"
git checkout main

echo -e "${GREEN}✅ Backup creado en rama: $backup_branch${NC}"

# Obtener PRs
prs=$(gh pr list --state open --json number,title,headRefName,author --limit 100)

if [ "$prs" = "[]" ]; then
    echo -e "${GREEN}✅ No hay pull requests abiertos${NC}"
    exit 0
fi

pr_count=$(echo "$prs" | jq length)
echo -e "${YELLOW}📊 Procesando $pr_count pull request(s)${NC}"

# Procesar cada PR de forma EXTREMADAMENTE agresiva
echo "$prs" | jq -r '.[] | "\(.number)|\(.title)|\(.headRefName)|\(.author.login)"' | while IFS='|' read -r pr_number pr_title head_ref author; do
    echo -e "${BLUE}🔥 SOBRESCRIBIENDO CON PR #$pr_number: '$pr_title' por @$author${NC}"
    
    # Fetch del PR
    git fetch origin "refs/pull/$pr_number/head:pr-$pr_number" 2>/dev/null || {
        echo -e "${RED}❌ Error al obtener PR #$pr_number${NC}"
        continue
    }
    
    # SOBRESCRITURA TOTAL: resetear main al contenido del PR
    echo -e "${RED}💥 SOBRESCRIBIENDO MAIN COMPLETAMENTE...${NC}"
    git reset --hard "pr-$pr_number"
    
    # Force push
    echo -e "${YELLOW}📤 Force pushing a main...${NC}"
    git push --force origin main
    
    # Cerrar PR
    gh pr close "$pr_number" --comment "🔥 SOBRESCRITO COMPLETAMENTE - Main ahora tiene el contenido exacto de este PR"
    
    echo -e "${GREEN}✅ Main sobrescrito con contenido del PR #$pr_number${NC}"
    echo ""
    
    # Limpiar
    git branch -D "pr-$pr_number" 2>/dev/null || true
done

echo -e "${GREEN}🎉 PROCESO COMPLETADO${NC}"
echo -e "${BLUE}📋 Main ha sido sobrescrito con el contenido del último PR procesado${NC}"
echo -e "${YELLOW}🔙 Si necesitas restaurar, usa la rama de backup: $backup_branch${NC}"
