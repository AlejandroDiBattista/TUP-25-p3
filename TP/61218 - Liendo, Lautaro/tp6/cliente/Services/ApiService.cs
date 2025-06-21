using System.Net.Http.Json;
using cliente.Models;
using cliente.DTOs;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly CartStateService _cartStateService;

        public ApiService(HttpClient httpClient, CartStateService cartStateService)
        {
            _httpClient = httpClient;
            _cartStateService = cartStateService;
        }

        public async Task<List<ArticuloInventario>?> GetProductosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("productos");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ArticuloInventario>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ArticuloInventario>?> SearchProductosAsync(string searchText)
        {
            try
            {
                var encodedSearchText = Uri.EscapeDataString(searchText);
                var response = await _httpClient.GetAsync($"productos/buscar/{encodedSearchText}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ArticuloInventario>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al buscar productos: {ex.Message}");
                return null;
            }
        }


        public async Task<string?> CreateCarritoAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("carritos", null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (result != null && result.TryGetValue("identificadorNuevoCarrito", out var cartId))
                {
                    _cartStateService.SetCartId(cartId);
                    return cartId;
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al crear carrito: {ex.Message}");
                return null;
            }
        }

        public async Task<List<DetalleCarritoMemoria>?> GetCarritoItemsAsync(string cartId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"carritos/{cartId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<DetalleCarritoMemoria>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener ítems del carrito: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddItemToCarritoAsync(string cartId, int articuloId, int cantidad)
        {
            try
            {

                var response = await _httpClient.PutAsync(
                    $"carritos/{cartId}/anadir/{articuloId}?cantidadSolicitada={cantidad}", null
                );


                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

                    if (errorResponse != null && errorResponse.TryGetValue("error", out var errorMsg))
                    {
                        throw new Exception(errorMsg);
                    }

                    throw new HttpRequestException($"Fallo al añadir item: {response.StatusCode}");
                }

                var nuevosItems = await response.Content.ReadFromJsonAsync<List<DetalleCarritoMemoria>>();

                if (nuevosItems != null)
                {
                    _cartStateService.SetCartItems(nuevosItems);
                }

                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[API] Error al añadir al carrito: {ex.Message}");
                throw;
            }
        }





        public async Task<bool> RemoveItemFromCarritoAsync(string cartId, int articuloId, int cantidad = 0)
        {
            try
            {
                string url = $"carritos/{cartId}/remover/{articuloId}";
                if (cantidad > 0)
                {
                    url += $"?cantidadAReducir={cantidad}";
                }
                var response = await _httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                var updatedItems = await response.Content.ReadFromJsonAsync<List<DetalleCarritoMemoria>>();
                if (updatedItems != null)
                {
                    _cartStateService.SetCartItems(updatedItems);
                }
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al remover ítem del carrito: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ClearCarritoAsync(string cartId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"carritos/{cartId}");
                response.EnsureSuccessStatusCode();
                _cartStateService.SetCartItems(new List<DetalleCarritoMemoria>());
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al vaciar carrito: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ConfirmCompraAsync(string cartId, DatosClienteDTO clientData)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"carritos/{cartId}/confirmar", clientData);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al confirmar compra: {response.StatusCode} - {content}");
                    var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (errorResponse != null && errorResponse.TryGetValue("error", out var errorMessage))
                    {
                        Console.WriteLine($"Mensaje de error del backend: {errorMessage}");
                        throw new Exception(errorMessage);
                    }
                    throw new HttpRequestException($"La petición de confirmación no fue exitosa: {response.StatusCode}");
                }

                _cartStateService.SetCartId(null);
                _cartStateService.SetCartItems(new List<DetalleCarritoMemoria>());
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de red al confirmar compra: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error lógico al confirmar compra: {ex.Message}");
                return false;
            }
        }
    }
}
