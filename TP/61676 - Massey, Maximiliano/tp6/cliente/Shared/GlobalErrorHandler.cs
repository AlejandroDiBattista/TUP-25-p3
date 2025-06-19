using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using cliente.Services;

namespace cliente.Shared;

public class GlobalErrorHandler : ErrorBoundary
{
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        // Mostrar notificación de error global
        NotificationService.ShowError(
            "Ha ocurrido un error inesperado. Por favor, recarga la página.",
            "Error de aplicación"
        );
        
        return Task.CompletedTask;
    }
}
