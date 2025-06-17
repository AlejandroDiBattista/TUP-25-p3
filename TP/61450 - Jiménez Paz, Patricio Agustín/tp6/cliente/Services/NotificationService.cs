using System;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class NotificationService
    {
        public class Notification
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Message { get; set; }
            public string Type { get; set; }
        }

        public event Action? OnNotifyChanged;
        private readonly List<Notification> _notifications = new();

        public IReadOnlyList<Notification> Notifications => _notifications;

        public void Show(string message, string type)
        {
            var notification = new Notification { Message = message, Type = type };
            _notifications.Add(notification);
            OnNotifyChanged?.Invoke();
        }

        public void Remove(Guid id)
        {
            var n = _notifications.FirstOrDefault(x => x.Id == id);
            if (n != null)
            {
                _notifications.Remove(n);
                OnNotifyChanged?.Invoke();
            }
        }

        public void ShowSuccess(string message) => Show(message, "success");
        public void ShowError(string message) => Show(message, "error");
        public void ShowInfo(string message) => Show(message, "info");
    }
}
