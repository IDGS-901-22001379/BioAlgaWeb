using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IPrecioService
    {
        Task<IReadOnlyList<PrecioDto>> GetHistorialAsync(int idProducto, CancellationToken ct = default);
        Task<IReadOnlyList<PrecioDto>> GetVigentesAsync(int idProducto, CancellationToken ct = default);
        Task<PrecioDto?> CrearAsync(int idProducto, CrearPrecioDto dto, CancellationToken ct = default);
        Task<bool> ActualizarAsync(int idPrecio, ActualizarPrecioDto dto, CancellationToken ct = default);
    }
}
