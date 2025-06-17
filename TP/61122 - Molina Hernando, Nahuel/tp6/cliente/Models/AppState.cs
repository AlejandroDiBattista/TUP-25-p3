
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

      public event Action OnProductosChange;

      private List<Producto> _productos = new List<Producto>();
      public List<Producto> Productos
      {
          get => _productos;
          set
          {
              if (_productos != value)
              {
                  _productos = value;
                  NotifyProductosStateChanged();
              }
          }
      }

      private void NotifyProductosStateChanged() => OnProductosChange?.Invoke();
  }
}