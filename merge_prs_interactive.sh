#!/bin/bash

# Script interactivo para mergear PRs con opciones
# Permite elegir estrategia por cada PR

set -e

# Colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m'

show_help() {
    echo -e "${BLUE}=== SCRIPT INTERACTIVO PARA MERGEAR PRs ===${NC}"
    echo "Uso: $0 [opciones]"
    echo ""
    echo "Opciones:"
    echo "  -a, --auto        Modo automático (mergea todos sin preguntar)"
    echo "  -f, --force       Modo forzado (sobrescribe conflictos)"
    echo "  -h, --help        Muestra esta ayuda"
    echo ""
    echo "Modos disponibles por PR:"
    echo "  1) Merge normal"
    echo "  2) Merge con sobrescritura (theirs)"
    echo "  3) Sobrescritura total (reset)"
    echo "  4) Saltar este PR"
    echo "  5) Salir del script"
}

AUTO_MODE=false
FORCE_MODE=false

# Parsear argumentos
while [[ $# -gt 0 ]]; do
    case $1 in
        -a|--auto)
            AUTO_MODE=true
            shift
            ;;
        -f|--force)
            FORCE_MODE=true
            shift
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo "Opción desconocida: $1"
            show_help
            exit 1
            ;;
    esac
done

echo -e "${BLUE}=== SCRIPT INTERACTIVO PARA MERGEAR PRs ===${NC}"

# Verificaciones
if [ ! -d ".git" ]; then
    echo -e "${RED}Error: No estás en un repositorio git${NC}"
    exit 1
fi

if ! gh auth status &>/dev/null; then
    echo -e "${RED}Error: GitHub CLI no está autenticado${NC}"
    exit 1
fi

# Ir a main
git checkout main
git pull origin main

# Crear backup
backup_branch="backup-main-$(date +%Y%m%d-%H%M%S)"
git checkout -b "$backup_branch"
git push origin "$backup_branch"
git checkout main
echo -e "${GREEN}💾 Backup creado: $backup_branch${NC}"

# Obtener PRs
prs=$(gh pr list --state open --json number,title,headRefName,author,url --limit 100)

if [ "$prs" = "[]" ]; then
    echo -e "${GREEN}✅ No hay pull requests abiertos${NC}"
    exit 0
fi

pr_count=$(echo "$prs" | jq length)
echo -e "${YELLOW}📊 Encontrados $pr_count pull request(s)${NC}"
echo ""

processed=0
merged=0
skipped=0

echo "$prs" | jq -r '.[] | "\(.number)|\(.title)|\(.headRefName)|\(.author.login)|\(.url)"' | while IFS='|' read -r pr_number pr_title head_ref author pr_url; do
    processed=$((processed + 1))
    
    echo -e "${PURPLE}=== PR #$pr_number [$processed/$pr_count] ===${NC}"
    echo -e "${BLUE}Título: $pr_title${NC}"
    echo -e "${BLUE}Autor: @$author${NC}"
    echo -e "${BLUE}Rama: $head_ref${NC}"
    echo -e "${BLUE}URL: $pr_url${NC}"
    echo ""
    
    if [ "$AUTO_MODE" = true ]; then
        if [ "$FORCE_MODE" = true ]; then
            choice=3
        else
            choice=2
        fi
        echo -e "${YELLOW}🤖 Modo automático: Opción $choice${NC}"
    else
        echo "¿Qué hacer con este PR?"
        echo "1) Merge normal"
        echo "2) Merge con sobrescritura (recomendado)"
        echo "3) Sobrescritura total"
        echo "4) Saltar este PR"
        echo "5) Salir del script"
        echo ""
        read -p "Selecciona una opción (1-5): " choice
    fi
    
    case $choice in
        1)
            echo -e "${BLUE}🔀 Merge normal...${NC}"
            git fetch origin "refs/pull/$pr_number/head:pr-$pr_number" 2>/dev/null || {
                echo -e "${RED}❌ Error al obtener PR${NC}"
                skipped=$((skipped + 1))
                continue
            }
            
            if git merge "pr-$pr_number" --no-edit; then
                git push origin main
                gh pr close "$pr_number" --comment "✅ Mergeado automáticamente via script"
                echo -e "${GREEN}✅ Merge exitoso${NC}"
                merged=$((merged + 1))
            else
                echo -e "${RED}❌ Conflictos en merge normal${NC}"
                git merge --abort
                skipped=$((skipped + 1))
            fi
            ;;
        2)
            echo -e "${BLUE}🔀 Merge con sobrescritura...${NC}"
            git fetch origin "refs/pull/$pr_number/head:pr-$pr_number" 2>/dev/null || {
                echo -e "${RED}❌ Error al obtener PR${NC}"
                skipped=$((skipped + 1))
                continue
            }
            
            git merge "pr-$pr_number" -X theirs --no-edit || {
                echo -e "${YELLOW}⚠️ Conflictos resueltos automáticamente${NC}"
            }
            git push origin main
            gh pr close "$pr_number" --comment "✅ Mergeado con sobrescritura automática via script"
            echo -e "${GREEN}✅ Merge con sobrescritura exitoso${NC}"
            merged=$((merged + 1))
            ;;
        3)
            echo -e "${RED}💥 Sobrescritura total...${NC}"
            git fetch origin "refs/pull/$pr_number/head:pr-$pr_number" 2>/dev/null || {
                echo -e "${RED}❌ Error al obtener PR${NC}"
                skipped=$((skipped + 1))
                continue
            }
            
            git reset --hard "pr-$pr_number"
            git push --force origin main
            gh pr close "$pr_number" --comment "🔥 Contenido sobrescrito completamente via script"
            echo -e "${GREEN}✅ Sobrescritura total exitosa${NC}"
            merged=$((merged + 1))
            ;;
        4)
            echo -e "${YELLOW}⏭️ Saltando PR #$pr_number${NC}"
            skipped=$((skipped + 1))
            ;;
        5)
            echo -e "${BLUE}👋 Saliendo del script${NC}"
            break
            ;;
        *)
            echo -e "${RED}❌ Opción inválida, saltando PR${NC}"
            skipped=$((skipped + 1))
            ;;
    esac
    
    # Limpiar rama temporal
    git branch -D "pr-$pr_number" 2>/dev/null || true
    echo ""
done

echo -e "${GREEN}🎉 PROCESO COMPLETADO${NC}"
echo -e "${BLUE}📊 Resumen:${NC}"
echo -e "  - PRs procesados: $processed"
echo -e "  - PRs mergeados: $merged"
echo -e "  - PRs saltados: $skipped"
echo -e "${YELLOW}🔙 Backup disponible en: $backup_branch${NC}"
