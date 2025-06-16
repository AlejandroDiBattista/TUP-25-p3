using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace cliente.Services;

public enum NotificationType
{
    Success,
    Error,
    Warning,
    Info
}

public class NotificationItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int Duration { get; set; } = 4000; // 4 segundos por defecto
    public string Icon => Type switch
    {
        NotificationType.Success => "✅",
        NotificationType.Error => "❌",
        NotificationType.Warning => "⚠️",
        NotificationType.Info => "ℹ️",
        _ => "ℹ️"
    };
    
    public string CssClass => Type switch
    {
        NotificationType.Success => "notification-success",
        NotificationType.Error => "notification-error", 
        NotificationType.Warning => "notification-warning",
        NotificationType.Info => "notification-info",
        _ => "notification-info"
    };
}

public class NotificationService
{
    private readonly List<NotificationItem> _notifications = new();
    private readonly IJSRuntime _jsRuntime;
    
    public NotificationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public event Action OnChange;
    public event Action OnCartUpdated; // Nuevo evento para actualización del carrito
    
    public IReadOnlyList<NotificationItem> Notifications => _notifications.AsReadOnly();
    
    public void ShowSuccess(string message, string title = "¡Éxito!", int duration = 4000)
    {
        AddNotification(title, message, NotificationType.Success, duration);
    }
    
    public void ShowError(string message, string title = "Error", int duration = 6000)
    {
        AddNotification(title, message, NotificationType.Error, duration);
    }
    
    public void ShowWarning(string message, string title = "Advertencia", int duration = 5000)
    {
        AddNotification(title, message, NotificationType.Warning, duration);
    }
    
    public void ShowInfo(string message, string title = "Información", int duration = 4000)
    {
        AddNotification(title, message, NotificationType.Info, duration);
    }
    
    private void AddNotification(string title, string message, NotificationType type, int duration)
    {
        var notification = new NotificationItem
        {
            Title = title,
            Message = message,
            Type = type,
            Duration = duration
        };
        
        _notifications.Add(notification);
        OnChange?.Invoke();
        
        // Auto-remover después del tiempo especificado
        _ = Task.Delay(duration).ContinueWith(_ => RemoveNotification(notification.Id));
    }
    
    public void RemoveNotification(string id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        if (notification != null)
        {
            _notifications.Remove(notification);
            OnChange?.Invoke();
        }
        }
    
    public void Clear()
    {
        _notifications.Clear();
        OnChange?.Invoke();
    }
    
    // Método para disparar actualización manual del carrito
    public async Task TriggerCartUpdateAsync()
    {
        OnCartUpdated?.Invoke();
        try
        {
            await _jsRuntime.InvokeVoidAsync("updateCartCounter");
        }
        catch (Exception)
        {
            // Ignorar errores de JavaScript en caso de que no esté disponible
        }
    }
      // === MÉTODOS ESPECÍFICOS PARA E-COMMERCE ===
    
    public void ShowProductAdded(string productName, int quantity = 1)
    {
        var message = quantity == 1 
            ? $"Has agregado {productName} a tu carrito"
            : $"Has agregado {quantity} unidades de {productName} a tu carrito";
            
        ShowSuccess(message, "¡Producto agregado! 🛒");
        OnCartUpdated?.Invoke();
        
        // Llamar versión asíncrona en segundo plano
        Task.Run(async () =>
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("updateCartCounter");
            }
            catch (Exception)
            {
                // Ignorar errores de JavaScript
            }
        });
    }
    
    public async Task ShowProductAddedAsync(string productName, int quantity = 1)
    {
        var message = quantity == 1 
            ? $"Has agregado {productName} a tu carrito"
            : $"Has agregado {quantity} unidades de {productName} a tu carrito";
            
        ShowSuccess(message, "¡Producto agregado! 🛒");
        OnCartUpdated?.Invoke();
        try
        {
            await _jsRuntime.InvokeVoidAsync("updateCartCounter");
        }
        catch (Exception)
        {
            // Ignorar errores de JavaScript
        }
    }
    
    public void ShowProductRemoved(string productName)
    {
        ShowInfo(
            $"{productName} removido del carrito",
            "🗑️ Producto eliminado",
            3000
        );
        OnCartUpdated?.Invoke();
        
        // Llamar versión asíncrona en segundo plano
        Task.Run(async () =>
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("updateCartCounter");
            }
            catch (Exception)
            {
                // Ignorar errores de JavaScript
            }
        });
    }
    
    public async Task ShowProductRemovedAsync(string productName)
    {
        ShowInfo(
            $"{productName} removido del carrito",
            "🗑️ Producto eliminado",
            3000
        );
        OnCartUpdated?.Invoke();
        try
        {
            await _jsRuntime.InvokeVoidAsync("updateCartCounter");
        }
        catch (Exception)
        {
            // Ignorar errores de JavaScript
        }
    }
    
    public void ShowOutOfStock(string productName)
    {
        ShowWarning($"No hay suficiente stock disponible para {productName}", "Stock insuficiente ⚠️");
    }    public void ShowPurchaseCompleted(decimal total, int itemCount)
    {
        ShowSuccess(
            $"¡Tu compra de {itemCount} producto{(itemCount > 1 ? "s" : "")} por ${total:F2} fue procesada exitosamente! Serás redirigido a la tienda...",
            "🎉 ¡Compra confirmada!",
            8000
        );
    }
    
    public void ShowConnectionError()
    {
        ShowError(
            "No se pudo conectar con el servidor. Verifica tu conexión a internet.",
            "Error de conexión 🌐",
            8000
        );
    }
    
    public void ShowWelcome(string userName = "")
    {
        var message = string.IsNullOrEmpty(userName) 
            ? "¡Bienvenido a nuestra tienda de juegos digitales!"
            : $"¡Bienvenido de vuelta, {userName}!";
            
        ShowInfo(message, "¡Hola! 👋", 6000);
    }
      // Validaciones específicas para e-commerce - métodos esenciales solamente
    
    public void ValidateFormFields(bool isValid, string fieldName = "")
    {
        if (!isValid)
        {
            ShowError(
                $"Por favor, completa correctamente el campo {fieldName}.",
                "Formulario incompleto"
            );
        }
    }
    
    public void ShowNetworkRetry(string operation = "operación")
    {
        ShowWarning(
            $"Reintentando {operation}. Si el problema persiste, verifica tu conexión.",
            "Reintentando...",
            6000
        );
    }
}
