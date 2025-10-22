using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface ICorteService
    {
        Task<CorteResumenDto?> ObtenerResumenTurnoAsync(int idTurno);
    }
}
