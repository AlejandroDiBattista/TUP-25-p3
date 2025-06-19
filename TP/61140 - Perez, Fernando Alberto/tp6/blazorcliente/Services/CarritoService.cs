using System.Threading.Tasks;

namespace TP._61140___Perez__Fernando_Alberto.tp6.blazorcliente.Services
{
    public class CarritoService
    {
        public event Action? OnChange;
        private int _cantidad = 0;
        public int Cantidad => _cantidad;

        public void SetCantidad(int cantidad)
        {
            _cantidad = cantidad;
            OnChange?.Invoke();
        }
    }
}
