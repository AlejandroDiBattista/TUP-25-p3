using System;
using System.Threading.Tasks;

namespace Cliente.Services
{
    public class FiltroService
    {
        public string Filtro { get; private set; } = "";

        public event Action? OnFiltroChanged;

        public void SetFiltro(string valor)
        {
            Filtro = valor;
            OnFiltroChanged?.Invoke();
        }
    }
}