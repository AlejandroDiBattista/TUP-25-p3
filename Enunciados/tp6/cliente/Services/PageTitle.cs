namespace cliente.Services;

/// <summary>
/// Servicio para gestionar el título de las páginas
/// </summary>
public class PageTitle
{
    private string _title = string.Empty;

    /// <summary>
    /// Evento que se dispara cuando cambia el título
    /// </summary>
    public event Action OnTitleChanged;

    /// <summary>
    /// Título actual de la página
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            OnTitleChanged?.Invoke();
        }
    }
}
