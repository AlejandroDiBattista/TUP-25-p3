using System;
using System.Collections.Concurrent;

namespace servidor.Models
{
    public class StockReserva
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime Expiracion { get; set; }
    }

    public class GestorStockReserva
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, StockReserva>> _reservas = new();
        private static readonly TimeSpan TIEMPO_EXPIRACION = TimeSpan.FromMinutes(30);

        public static void AgregarReserva(int productoId, Guid carritoId, int cantidad)
        {
            LimpiarReservasExpiradas();
            
            var reservasProducto = _reservas.GetOrAdd(productoId, _ => new ConcurrentDictionary<Guid, StockReserva>());
            
            reservasProducto[carritoId] = new StockReserva
            {
                ProductoId = productoId,
                Cantidad = cantidad,
                Expiracion = DateTime.UtcNow.Add(TIEMPO_EXPIRACION)
            };
        }

        public static void EliminarReserva(int productoId, Guid carritoId)
        {
            if (_reservas.TryGetValue(productoId, out var reservasProducto))
            {
                reservasProducto.TryRemove(carritoId, out _);
                if (reservasProducto.IsEmpty)
                    _reservas.TryRemove(productoId, out _);
            }
        }

        public static int ObtenerTotalReservado(int productoId)
        {
            LimpiarReservasExpiradas();
            
            if (_reservas.TryGetValue(productoId, out var reservasProducto))
            {
                return reservasProducto.Values.Sum(r => r.Cantidad);
            }
            return 0;
        }

        private static void LimpiarReservasExpiradas()
        {
            var ahora = DateTime.UtcNow;
            foreach (var productoReservas in _reservas)
            {
                foreach (var reserva in productoReservas.Value)
                {
                    if (reserva.Value.Expiracion < ahora)
                    {
                        productoReservas.Value.TryRemove(reserva.Key, out _);
                    }
                }

                if (productoReservas.Value.IsEmpty)
                {
                    _reservas.TryRemove(productoReservas.Key, out _);
                }
            }
        }
    }
}