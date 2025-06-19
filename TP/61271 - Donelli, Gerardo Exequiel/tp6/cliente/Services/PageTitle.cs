using Microsoft.AspNetCore.Components;

namespace cliente.Services;

/// <summary>
/// Servicio para gestionar el título de las páginas
/// </summary>
public class PageTitleService
{
    private readonly ILogger<PageTitleService> _logger;
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
            if (_title != value)
            {
                _title = value;
                NotifyStateChanged();
            }
        }
    }

    private void NotifyStateChanged() => OnTitleChanged?.Invoke();
}
