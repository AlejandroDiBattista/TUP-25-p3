using cliente.Modelos;

namespace cliente.Services
{
    public class CarritoService
    {
        public int CarritoId { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
        public event Action? OnChange;
        public void NotificarCambio() => OnChange?.Invoke();
        public int Contador => Items.Sum(i => i.Cantidad);

        public void Limpiar()
        {
            Items.Clear();
            CarritoId = 0;
        }
    }
}