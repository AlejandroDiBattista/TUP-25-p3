using Microsoft.AspNetCore.Components;
using cliente.Services;

namespace cliente.Components;

public class LoadingStateComponent : ComponentBase
{
    [Inject] protected NotificationService NotificationService { get; set; } = default!;
    
    protected bool IsLoading { get; set; }
    protected string? ErrorMessage { get; set; }
    
    protected async Task ExecuteWithLoadingAsync(Func<Task> operation, string? loadingMessage = null, string? successMessage = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            StateHasChanged();
            
            if (!string.IsNullOrEmpty(loadingMessage))
            {
                NotificationService.ShowInfo(loadingMessage, "Procesando...");
            }
            
            await operation();
            
            if (!string.IsNullOrEmpty(successMessage))
            {
                NotificationService.ShowSuccess(successMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.ShowError(
                "Ocurrió un error inesperado. Por favor intenta nuevamente.",
                "Error"
            );
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
    
    protected async Task<T?> ExecuteWithLoadingAsync<T>(Func<Task<T>> operation, string? loadingMessage = null, string? successMessage = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            StateHasChanged();
            
            if (!string.IsNullOrEmpty(loadingMessage))
            {
                NotificationService.ShowInfo(loadingMessage, "Procesando...");
            }
            
            var result = await operation();
            
            if (!string.IsNullOrEmpty(successMessage))
            {
                NotificationService.ShowSuccess(successMessage);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.ShowError(
                "Ocurrió un error inesperado. Por favor intenta nuevamente.",
                "Error"
            );
            return default;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}
