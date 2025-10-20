using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface ICajaMovimientoRepository
    {
        Task<(IReadOnlyList<CajaMovimiento> Items, int Total)> BuscarPorTurnoAsync(
            int idTurno, string? tipo, string? qConcepto, int page = 1, int pageSize = 20);

        Task<CajaMovimiento?> GetByIdAsync(int id);
        Task<CajaMovimiento> AddAsync(CajaMovimiento mov);
        Task<bool> UpdateAsync(CajaMovimiento mov);
        Task<bool> DeleteAsync(int id);
    }
}
