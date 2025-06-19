using System;
using System.Threading.Tasks;

public class SearchService
{
    #nullable enable
    public string TerminoBusqueda { get; private set; } = "";

    public event Func<string, Task>? OnBuscar;

    public async Task Buscar(string termino)
    {
        TerminoBusqueda = termino;
        if (OnBuscar is not null)
            await OnBuscar.Invoke(termino);
    }
}
