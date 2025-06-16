using System.Net.Http.Json;
using cliente.Modelos;

public class Buscador
{
    private string _termino = "";

    public string Termino
    {
        get => _termino;
        set
        {
            if (_termino != value)
            {
                _termino = value;
                NotifyTerminoChanged?.Invoke(_termino);
            }
        }
    }

    public event Action<string>? NotifyTerminoChanged;
}
