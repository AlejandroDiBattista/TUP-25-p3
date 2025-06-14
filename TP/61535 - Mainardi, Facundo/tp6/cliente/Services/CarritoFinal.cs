namespace Cliente.Services
{
    public class CarritoFinal
    {
        public event Action OnChange;

        private List<ItemCarrito> items = new();

        public IReadOnlyList<ItemCarrito> Items => items;

        public int TotalItems => items.Sum(i => i.Cantidad);

        public void Agregar(ItemCarrito item)
        {
            var existente = items.FirstOrDefault(i => i.ProductoId == item.ProductoId);
            if (existente != null)
            {
                existente.Cantidad += item.Cantidad;
            }
            else
            {
                items.Add(item);
            }

            OnChange?.Invoke();
        }

        public void Vaciar()
        {
            items.Clear();
            OnChange?.Invoke();
        }
    }

    public class ItemCarrito
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}