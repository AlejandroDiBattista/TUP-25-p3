#!/bin/bash

# Script para listar todos los pull requests que modificaron calculadora.html en tp7
# Autor: Script generado para análisis de TP7
# Fecha: $(date +"%Y-%m-%d %H:%M:%S")

echo "=== Pull Requests que modificaron calculadora.html en tp7 ==="
echo "Repositorio: $(git remote get-url origin 2>/dev/null || echo 'Repositorio local')"
echo "Fecha de análisis: $(date)"
echo ""

# Función para extraer el número de PR del mensaje de commit
extraer_numero_pr() {
    local mensaje="$1"
    echo "$mensaje" | grep -o '#[0-9]\+' | head -1 | tr -d '#'
}

# Función para obtener el título del PR desde el mensaje de commit
obtener_titulo_pr() {
    local mensaje="$1"
    # Remover el hash del PR del final
    echo "$mensaje" | sed 's/ (#[0-9]\+)$//' | sed 's/^[a-f0-9]\+ //'
}

# Buscar commits que mencionan tanto tp7 como calculadora.html
echo "Buscando commits relacionados con tp7 y calculadora.html..."
echo ""

# Obtener commits que modificaron archivos calculadora.html en carpetas tp7
git log --oneline --all --name-only --pretty=format:"%h|%s" | \
while IFS='|' read -r hash mensaje || IFS='|' read -r archivo _; do
    if [[ "$archivo" == *"tp7"* && "$archivo" == *"calculadora.html"* ]]; then
        # Si encontramos un archivo que coincide, obtener el mensaje del commit anterior
        if [[ -n "$hash" && -n "$mensaje" ]]; then
            numero_pr=$(extraer_numero_pr "$mensaje")
            titulo=$(obtener_titulo_pr "$mensaje")
            
            if [[ -n "$numero_pr" ]]; then
                echo "PR #$numero_pr: $titulo"
                echo "  Commit: $hash"
                echo "  Archivo: $archivo"
                echo ""
            else
                echo "$titulo"
                echo " - $archivo"
                echo ""
            fi
        fi
    fi
done

echo ""
echo "=== Método alternativo: Buscar por patrones en mensajes de commit ==="
echo ""

# Buscar commits con patrones específicos
# git log --oneline --all --grep="tp7.*calculadora" --grep="calculadora.*tp7" --grep="TP7.*[Cc]alculadora" --grep="[Cc]alculadora.*TP7" | \
# while read -r linea; do
#     hash=$(echo "$linea" | cut -d' ' -f1)
#     mensaje=$(echo "$linea" | cut -d' ' -f2-)
#     numero_pr=$(extraer_numero_pr "$mensaje")
#     titulo=$(obtener_titulo_pr "$mensaje")
    
#     if [[ -n "$numero_pr" ]]; then
#         echo "PR #$numero_pr: $titulo"
#     else
#         echo "Commit: $titulo"
#     fi
#     echo "  Hash: $hash"
#     echo ""
# done

echo ""
echo "=== Pull Requests encontrados por patrón tp7 ==="
echo ""

# Buscar todos los commits que mencionen tp7 en el mensaje
git log --oneline --all --grep="tp7" --grep="TP7" | \
grep -i "calculadora\|#" | \
while read -r linea; do
    hash=$(echo "$linea" | cut -d' ' -f1)
    mensaje=$(echo "$linea" | cut -d' ' -f2-)
    numero_pr=$(extraer_numero_pr "$mensaje")
    titulo=$(obtener_titulo_pr "$mensaje")
    
    # Verificar si el commit realmente modificó calculadora.html en tp7
    archivos_modificados=$(git show --name-only "$hash" | grep "tp7.*calculadora.html")
    
    if [[ -n "$archivos_modificados" ]]; then
        echo "$archivos_modificados" | sed 's/^/    /'
    fi
done

