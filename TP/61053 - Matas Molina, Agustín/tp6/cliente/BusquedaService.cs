public class BusquedaService
{
    public string TextoBusqueda { get; set; } = "";
    public event Action? OnChange;

    public void SetTexto(string texto)
    {
        TextoBusqueda = texto;
        OnChange?.Invoke();
    }
}