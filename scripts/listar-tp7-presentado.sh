#!/bin/bash

# Script para listar todos los pull requests que modificaron calculadora.html en tp7
# Autor: Script generado para análisis de TP7
# Fecha: $(date +"%Y-%m-%d %H:%M:%S")

# Lista temporal para evitar duplicados (usaremos archivos temporales)
TEMP_COMMITS="/tmp/commits_tp7_$(date +%s).txt"
touch "$TEMP_COMMITS"

echo "=== MÉTODO 1: Búsqueda exhaustiva por archivos modificados ==="
echo "Buscando todos los commits que modificaron calculadora.html en carpetas tp7..."
echo ""

# Buscar directamente commits que modificaron archivos calculadora.html en tp7
current_hash=""
current_mensaje=""

git log --all --name-only --pretty=format:"%H|%s" --grep="" -- "**/tp7/**/calculadora.html" "**/tp7/calculadora.html" "tp7/**/calculadora.html" "tp7/calculadora.html" 2>/dev/null | \
while read -r linea; do
    if [[ "$linea" == *"|"* ]]; then
        # Es una línea de commit (hash|mensaje)
        current_hash=$(echo "$linea" | cut -d'|' -f1)
        current_mensaje=$(echo "$linea" | cut -d'|' -f2)
    elif [[ "$linea" == *"tp7"* && "$linea" == *"calculadora.html"* ]]; then
        # Es un archivo que coincide con nuestro patrón
        if [[ -n "$current_hash" ]] && ! grep -q "$current_hash" "$TEMP_COMMITS" 2>/dev/null; then
            echo "$current_hash" >> "$TEMP_COMMITS"
            archivos_tp7=$(git show --name-only "$current_hash" | grep -E "tp7.*calculadora\.html")
            if [[ -n "$archivos_tp7" ]]; then
                mostrar_resultado "$current_hash" "$current_mensaje" "$archivos_tp7"
            fi
        fi
    fi
done
