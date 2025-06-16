using cliente.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cliente.Services
{
    public class ProductoService
    {
        private readonly ApiService _apiService;
        public event Action? OnSearchUpdated;

        private List<Producto> productos = new();
        public List<Producto> ProductosFiltrados { get; private set; } = new();

        public ProductoService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task CargarProductos()
        {
            var lista = await _apiService.ObtenerProductosAsync();
            productos = lista ?? new List<Producto>();

            ProductosFiltrados = new List<Producto>(productos);
            OnSearchUpdated?.Invoke();
        }

        public void FiltrarProductos(string criterio)
        {
            if (string.IsNullOrEmpty(criterio))
            {
                ProductosFiltrados = new List<Producto>(productos);
            }
            else
            {
                ProductosFiltrados = productos.FindAll(p => p.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase));
            }
            OnSearchUpdated?.Invoke();
        }
    }
}
