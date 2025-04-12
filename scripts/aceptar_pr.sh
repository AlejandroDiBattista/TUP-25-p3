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

    # Verificar si se modificó únicamente "ejercicio.cs"
    FILE_COUNT=$(echo "$MODIFIED_FILES" | wc -l)
    if [ "$FILE_COUNT" -eq 1 ] && echo "$MODIFIED_FILES" | grep -Fxq "ejercicio.cs"; then
        echo "Aceptando el pull request #$PR_NUMBER porque solo modificó ejercicio.cs"
        gh pr merge $PR_NUMBER --repo $REPO --squash --delete-branch
    else
        echo "Error: El pull request #$PR_NUMBER modificó otros archivos o más de un archivo: $MODIFIED_FILES"
    fi

done