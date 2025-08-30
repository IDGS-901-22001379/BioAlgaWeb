using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IInventarioRepository
    {
        /// <summary>
        /// Agrega un movimiento de inventario (Entrada, Salida o Ajuste).
        /// No guarda cambios hasta que se invoque GuardarCambiosAsync.
        /// </summary>
        Task AddMovimientoAsync(InventarioMovimiento mov, CancellationToken ct = default);

        /// <summary>
        /// Guarda los cambios pendientes en la base de datos.
        /// </summary>
        Task GuardarCambiosAsync(CancellationToken ct = default);

        /// <summary>
        /// Calcula el stock actual de un producto.
        /// </summary>
        Task<int> StockActualAsync(int idProducto, CancellationToken ct = default);
    }
}
