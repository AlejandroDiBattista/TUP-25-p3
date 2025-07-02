#!/bin/bash

# Obtener todos los pull requests abiertos
PR_LIST=$(gh pr list --state open --json number --jq '.[].number')

# Verificar si hay pull requests abiertos
if [ -z "$PR_LIST" ]; then
    echo "No hay pull requests abiertos."
    exit 0
fi

REPO=$(gh repo view --json nameWithOwner --jq '.nameWithOwner' 2>/dev/null)

# Verificar si el repositorio está configurado
if [ -z "$REPO" ]; then
    echo "Error: No se pudo obtener el repositorio actual. Asegúrate de que el CLI de GitHub esté configurado."
    exit 1
fi

# Iterar sobre cada pull request
for PR_NUMBER in $PR_LIST; do
    echo "Procesando pull request #$PR_NUMBER..."

    # Obtener los archivos modificados en el pull request
    MODIFIED_FILES=$(gh pr view $PR_NUMBER --repo $REPO --json files --jq '.files[].path' 2>/dev/null)

    # Verificar si hubo un error al obtener los archivos
    if [ -z "$MODIFIED_FILES" ]; then
        echo "Error: No se pudo obtener información del pull request #$PR_NUMBER."
        continue
    fi

    # Contar archivos modificados
    FILE_COUNT=$(echo "$MODIFIED_FILES" | wc -l)
    
    # Verificar si se modificó únicamente un archivo
    if [ "$FILE_COUNT" -eq 1 ]; then
        # Obtener el nombre del archivo (sin la ruta)
        FILE_NAME=$(basename "$MODIFIED_FILES")
        
        # Verificar si el archivo es calculadora.html
        if [ "$FILE_NAME" = "calculadora.html" ]; then
            echo "✅ Pull request #$PR_NUMBER modifica únicamente calculadora.html. Aceptando..."
            gh pr merge $PR_NUMBER --repo $REPO --squash --delete-branch
            
            if [ $? -eq 0 ]; then
                echo "✅ Pull request #$PR_NUMBER aceptado exitosamente."
            else
                echo "❌ Error al aceptar el pull request #$PR_NUMBER."
            fi
        else
            echo "❌ Pull request #$PR_NUMBER modifica '$FILE_NAME', no 'calculadora.html'. Saltando..."
        fi
    else
        echo "❌ Pull request #$PR_NUMBER modifica $FILE_COUNT archivos. Solo se aceptan PR que modifiquen un único archivo. Saltando..."
    fi

done

echo "Procesamiento completado."
