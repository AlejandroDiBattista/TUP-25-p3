using System;

public class BusquedaService
{
    private string _terminoBusqueda = "";

    public string TerminoBusqueda
    {
        get => _terminoBusqueda;
        set
        {
            if (_terminoBusqueda != value)
            {
                _terminoBusqueda = value;
                OnBusquedaCambiada?.Invoke(_terminoBusqueda);
            }
        }
    }

    public event Action<string>? OnBusquedaCambiada;
}