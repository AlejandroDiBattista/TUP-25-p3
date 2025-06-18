#nullable enable
using Microsoft.Extensions.Options;
using Cliente.Constants;

namespace Cliente.Services
{
    public class ToastService
    {
        private readonly ToastDurationSettings _durations;
        
        public event Action<string, ToastType, int>? OnToastRequested;

        public ToastService(IOptions<AppSettings> appSettings)
        {
            _durations = appSettings.Value.ToastDuration;
        }
        
        public void ShowSuccess(string message, int? duration = null)
        {
            OnToastRequested?.Invoke(message, ToastType.Success, duration ?? _durations.Success);
        }
        
        public void ShowError(string message, int? duration = null)
        {
            OnToastRequested?.Invoke(message, ToastType.Error, duration ?? _durations.Error);
        }
        
        public void ShowWarning(string message, int? duration = null)
        {
            OnToastRequested?.Invoke(message, ToastType.Warning, duration ?? _durations.Warning);
        }
        
        public void ShowInfo(string message, int? duration = null)
        {
            OnToastRequested?.Invoke(message, ToastType.Info, duration ?? _durations.Info);
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
