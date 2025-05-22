#!/usr/bin/env python3
# Requiere mcp[cli] ≥ 1.2   →  pip3 install "mcp[cli]"

import asyncio
from mcp import ClientSession, StdioServerParameters
from mcp.client.stdio import stdio_client

async def main():
    server_params = StdioServerParameters(
        command="python3",
        args=["21.1-servidor.py"],
    )

    async with stdio_client(server_params) as (read, write):
        async with ClientSession(read, write) as session:
            await session.initialize()

            print("=== Herramientas disponibles ===")
            for tool in await session.list_tools():
                if tool[0] != "tools": continue
                for t in tool[1]:
                    properties = t.inputSchema.get("properties", {})
                    parametros = [f"{v.get('type')} {k}" for k, v in properties.items()]
                    print(f"✧ {t.name}({', '.join(parametros)})\n  '{t.description}'\n")

            async def factorial(n: int):
                res = await session.call_tool("factorial", {"numero": n})
                return int(res.content[0].text)

            async def primeros_primos(n: int):
                res = await session.call_tool("primeros_primos", {"n": n})
                return [int(i.text) for i in res.content]

            print(f"\nResultado Factorial(10) = {await factorial(10)}")
            print(f"\nPrimeros Primos(10)     = {await primeros_primos(10)}")


if __name__ == "__main__":
    asyncio.run(main())