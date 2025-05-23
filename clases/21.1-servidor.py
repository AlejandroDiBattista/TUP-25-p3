#!/usr/bin/env python3
# Requiere mcp[cli] ≥ 1.2   →  pip3 install "mcp[cli]"

from typing import List
from mcp.server.fastmcp import FastMCP 
from itertools import islice, count

mcp = FastMCP("calculo")   # nombre lógico del servidor/raíz

@mcp.tool(description="Calcula el factorial de un número entero no negativo.")
def factorial(numero: int) -> int:
    """Devuelve n! para n ≥ 0."""
    if numero < 0:
        raise ValueError("El número debe ser no negativo.")
    resultado = 1
    for i in range(2, numero + 1):
        resultado *= i
    return resultado

@mcp.tool(description="Devuelve una lista con los n primeros números primos.")
def primeros_primos(n: int) -> List[int]:
    """Lista de los n primos iniciales (n > 0)."""
    
    def es_primo(numero: int) -> bool:
        if numero < 2      : return False
        if numero == 2     : return True
        if numero % 2 == 0 : return False
        
        limite = int(numero ** 0.5)
        return all(numero % i != 0 for i in range(3, limite + 1, 2))

    if n <= 0: raise ValueError("El valor debe ser mayor que cero.")
    primos = list(islice((x for x in count(2) if es_primo(x)), n))
    return primos


if __name__ == "__main__":
    # Ejecuta el servidor usando transporte STDIO
    mcp.run(transport="stdio")               #  [oai_citation:1‡modelcontextprotocol.io](https://modelcontextprotocol.io/quickstart/server)