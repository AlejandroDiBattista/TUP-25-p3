
namespace cliente.Models
{
  public class AppState
  {
      public event Action OnContadorChange;

      private int _contadorCarrito;
      public int ContadorCarrito
      {
          get => _contadorCarrito;
          set
          {
              if (_contadorCarrito != value)
              {
                  _contadorCarrito = value;
                  NotifyContadorStateChanged();
              }
          }
      }

      private void NotifyContadorStateChanged() => OnContadorChange?.Invoke();
  }
}