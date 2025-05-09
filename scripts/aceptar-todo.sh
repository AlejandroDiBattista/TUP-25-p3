#!/bin/bash

# Obtener todos los pull requests abiertos
PR_LIST=$(gh pr list --state open --json number,mergeable --jq '.[]')

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
echo "$PR_LIST" | while read -r pr; do
    PR_NUMBER=$(echo "$pr" | jq -r '.number')
    MERGEABLE=$(echo "$pr" | jq -r '.mergeable')

    echo "Procesando pull request #$PR_NUMBER..."

    if [ "$MERGEABLE" = "MERGEABLE" ]; then
        echo "Aceptando el pull request #$PR_NUMBER ya que no tiene conflictos"
        gh pr merge $PR_NUMBER --repo $REPO --squash --delete-branch
    else
        echo "Error: El pull request #$PR_NUMBER tiene conflictos y no puede ser mergeado automáticamente"
    fi
done