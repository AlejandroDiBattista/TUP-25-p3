using System.Net.Http.Json; // Necesario para PutAsJsonAsync, GetFromJsonAsync
using cliente.Modelos; // Asegúrate de que esta ruta sea correcta para Producto y DatosCliente
using System.Linq; // Para FirstOrDefault, Sum
using System.Collections.Generic; // Para List

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    public string? carritoId { get; private set; } // Dejado en lowercase como lo tenías originalmente

    public List<Producto> ProductosEnCarrito { get; private set; } = new();

    public event Action? OnChange;

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    // Método para crear un nuevo carrito en el backend
    // Este método es CRÍTICO y debe ser llamado antes de agregar productos.
    public async Task CrearCarritoAsync()
    {
        var response = await _http.PostAsync("/carritos", null);
        if (response.IsSuccessStatusCode)
        {
            string rawCarritoId = await response.Content.ReadAsStringAsync();
            // --- INICIO DE CORRECCIÓN CRÍTICA ---
            carritoId = rawCarritoId.Trim('"'); // Recorta las comillas
            // --- FIN DE CORRECCIÓN CRÍTICA ---

            Console.WriteLine($"DEBUG: rawCarritoId antes de Trim: '{rawCarritoId}' (Longitud: {rawCarritoId.Length})");
            Console.WriteLine($"DEBUG: carritoId después de Trim: '{carritoId}' (Longitud: {carritoId.Length})");
            Console.WriteLine($"🛒 Carrito creado con ID: \"{carritoId}\"");

            ProductosEnCarrito.Clear(); // Limpia los ítems locales
            OnChange?.Invoke(); // Notifica a los componentes
        }
        else
        {
            Console.WriteLine($"Error al crear carrito: {response.StatusCode}");
            // Considera cómo manejar un fallo aquí, por ejemplo, notificando al usuario.
        }
    }

    // --- MANTENIENDO TU MÉTODO ORIGINAL DE AGREGAR, PERO SINCRONIZANDO CON BACKEND ---
    public async Task AgregarProductoAsync(Producto producto)
    {
        if (producto.Stock <= 0)
        {
            Console.WriteLine($"DEBUG: Producto {producto.Nombre} sin stock.");
            return;
        }

        // 1. Crear carrito si aún no existe
        if (carritoId == null)
        {
            await CrearCarritoAsync();
            if (carritoId == null) // Si falló al crear el carrito, salimos
            {
                Console.WriteLine("DEBUG: No se pudo crear el carrito. No se puede agregar el producto.");
                return;
            }
            Console.WriteLine($"DEBUG: Carrito creado (y ahora usado) con ID: {carritoId}");
        }

        // 2. Intentar agregar/actualizar en el BACKEND
        // El backend espera el ProductoId y la cantidad.
        // Aquí asumimos que cada llamada a AgregarProductoAsync suma 1 a la cantidad.
        // Primero, ver cuántos tenemos ya en el carrito local.
        var existenteLocal = ProductosEnCarrito.FirstOrDefault(p => p.Id == producto.Id);
        int nuevaCantidad = (existenteLocal?.Cantidad ?? 0) + 1;

        // Llama al endpoint PUT /carritos/{id}/{productoId}?cantidad={cantidad}
        var response = await _http.PutAsync($"/carritos/{carritoId}/{producto.Id}?cantidad={nuevaCantidad}", null);

        if (response.IsSuccessStatusCode)
        {
            // 3. Si tuvo éxito en el backend, actualiza la lista local
            if (existenteLocal != null)
            {
                existenteLocal.Cantidad = nuevaCantidad; // Actualiza la cantidad
            }
            else
            {
                // Agrega el producto nuevo al carrito local con Cantidad = 1 (para esta operación)
                // Es importante que el objeto Producto aquí refleje las propiedades que el backend espera
                // en su CarritoItem (ProductoId, Nombre, PrecioUnitario, Cantidad).
                // Tu modelo Producto ya tiene Id, Nombre, Precio. Usaremos esos.
                ProductosEnCarrito.Add(new Producto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock, // El stock se manejará en el backend
                    ImagenUrl = producto.ImagenUrl,
                    Cantidad = nuevaCantidad // Cantidad total actual para este producto
                });
            }

            OnChange?.Invoke(); // Notifica a los componentes
            Console.WriteLine($"DEBUG: Producto {producto.Nombre} agregado/actualizado en carrito. Nueva Cantidad: {nuevaCantidad}");
        }
        else
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error al agregar/actualizar producto en carrito (backend): {response.StatusCode} - {errorContent}");
            // Si el backend devuelve un error (ej. por stock), considera no modificar el carrito local.
        }
    }

    // --- MANTENIENDO TU MÉTODO ORIGINAL DE ELIMINAR, SINCRONIZANDO CON BACKEND ---
    public async Task EliminarProducto(int id)
    {
        if (carritoId == null)
        {
            Console.WriteLine("DEBUG: No hay carrito activo para eliminar producto.");
            return;
        }

        // 1. Eliminar del backend
        var response = await _http.DeleteAsync($"/carritos/{carritoId}/{id}");

        if (response.IsSuccessStatusCode)
        {
            // 2. Si tuvo éxito en el backend, elimina de la lista local
            var producto = ProductosEnCarrito.FirstOrDefault(p => p.Id == id);
            if (producto != null)
            {
                ProductosEnCarrito.Remove(producto);
            }
            OnChange?.Invoke();
            Console.WriteLine($"DEBUG: Producto con ID {id} eliminado del carrito.");
        }
        else
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error al eliminar producto del carrito (backend): {response.StatusCode} - {errorContent}");
        }
    }

    // --- MANTENIENDO TUS MÉTODOS DE CANTIDAD, PERO CONSINCRONIZACIÓN BÁSICA ---
    public async Task AumentarCantidad(int id)
    {
        var productoLocal = ProductosEnCarrito.FirstOrDefault(p => p.Id == id);
        if (productoLocal != null)
        {
            // Asumimos que el stock ya se redujo al agregar inicialmente, o que el backend lo controla.
            // Aquí simplemente aumentamos la cantidad y llamamos al backend con la nueva cantidad total.
            int nuevaCantidad = productoLocal.Cantidad + 1;

            // Llama al backend para actualizar la cantidad
            if (carritoId == null) return; // No hay carrito activo
            var response = await _http.PutAsync($"/carritos/{carritoId}/{productoLocal.Id}?cantidad={nuevaCantidad}", null);

            if (response.IsSuccessStatusCode)
            {
                productoLocal.Cantidad = nuevaCantidad; // Actualiza solo si el backend tuvo éxito
                OnChange?.Invoke();
                Console.WriteLine($"DEBUG: Cantidad de {productoLocal.Nombre} aumentada a {nuevaCantidad}.");
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al aumentar cantidad (backend): {response.StatusCode} - {errorContent}");
            }
        }
    }

    public async Task DisminuirCantidad(int id)
    {
        var productoLocal = ProductosEnCarrito.FirstOrDefault(p => p.Id == id);
        if (productoLocal != null)
        {
            if (productoLocal.Cantidad > 1)
            {
                int nuevaCantidad = productoLocal.Cantidad - 1;

                // Llama al backend para actualizar la cantidad
                if (carritoId == null) return; // No hay carrito activo
                var response = await _http.PutAsync($"/carritos/{carritoId}/{productoLocal.Id}?cantidad={nuevaCantidad}", null);

                if (response.IsSuccessStatusCode)
                {
                    productoLocal.Cantidad = nuevaCantidad; // Actualiza solo si el backend tuvo éxito
                    OnChange?.Invoke();
                    Console.WriteLine($"DEBUG: Cantidad de {productoLocal.Nombre} disminuida a {nuevaCantidad}.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al disminuir cantidad (backend): {response.StatusCode} - {errorContent}");
                }
            }
            else // Si la cantidad es 1 y se disminuye, eliminar el producto
            {
                await EliminarProducto(id); // Reutiliza tu método EliminarProducto
                Console.WriteLine($"DEBUG: Producto con ID {id} eliminado al disminuir cantidad a 0.");
            }
        }
    }


    // --- MANTENIENDO TU MÉTODO ORIGINAL DE LIMPIAR ---
    // Este método solo limpia la lista local, no el carrito en el backend.
    // Para limpiar el carrito del backend, necesitarías un endpoint y una llamada PUT/DELETE específica.
    public void LimpiarCarrito()
    {
        ProductosEnCarrito.Clear();
        OnChange?.Invoke();
    }

    public decimal CalcularTotal() =>
        ProductosEnCarrito.Sum(p => p.Precio * p.Cantidad);

    public int CantidadTotal => ProductosEnCarrito.Sum(p => p.Cantidad);


    public async Task<bool> ConfirmarCompraAsync(DatosCliente cliente)
    {
        if (carritoId == null)
        {
            return false;
        }


        var response = await _http.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", cliente);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Compra confirmada exitosamente.");
            LimpiarCarrito(); // Limpia la lista local
            carritoId = null; // Resetea el ID del carrito para futuras compras
            // OnChange?.Invoke(); // Ya invocado por LimpiarCarrito()
            return true;
        }
        else
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error al confirmar compra: {response.StatusCode} - {errorContent}");
            return false;
        }
    }
}