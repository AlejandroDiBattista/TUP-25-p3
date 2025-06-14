using Blazored.LocalStorage;
using cliente.Services;
using cliente.Models;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cliente.Shared
{
    public static class CarritoHelper
    {
        public static async Task<(Guid carritoId, List<ItemCarrito> items, bool recreated)> ObtenerOCrearCarritoValidoAsync(
            ILocalStorageService localStorage, ApiService apiService)
        {
            var carritoId = await localStorage.GetItemAsync<Guid?>("carritoId");
            if (carritoId == null || carritoId == Guid.Empty)
            {
                var nuevo = await apiService.CrearCarritoAsync();
                await localStorage.SetItemAsync("carritoId", nuevo.ToString());
                return (nuevo, new List<ItemCarrito>(), true);
            }

            try
            {
                var items = await apiService.ObtenerCarritoAsync(carritoId.Value);
                return (carritoId.Value, items, false);
            }
            catch (HttpRequestException)
            {
                var nuevo = await apiService.CrearCarritoAsync();
                await localStorage.SetItemAsync("carritoId", nuevo.ToString());
                return (nuevo, new List<ItemCarrito>(), true);
            }
        }
    }
}