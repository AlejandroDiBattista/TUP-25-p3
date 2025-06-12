#nullable enable

namespace Cliente.Services
{
    public class ToastService
    {
        public event Action<string, ToastType, int>? OnToastRequested;
        
        public void ShowSuccess(string message, int duration = 4000)
        {
            OnToastRequested?.Invoke(message, ToastType.Success, duration);
        }
        
        public void ShowError(string message, int duration = 5000)
        {
            OnToastRequested?.Invoke(message, ToastType.Error, duration);
        }
        
        public void ShowWarning(string message, int duration = 4000)
        {
            OnToastRequested?.Invoke(message, ToastType.Warning, duration);
        }
        
        public void ShowInfo(string message, int duration = 3000)
        {
            OnToastRequested?.Invoke(message, ToastType.Info, duration);
        }
    }
    
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
}
