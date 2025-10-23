using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface ICajaTurnoRepository
    {
        Task<CajaTurno?> GetByIdAsync(int id);

        Task<CajaTurno?> GetTurnoAbiertoPorCajaAsync(int idCaja);
        Task<CajaTurno?> GetTurnoAbiertoPorUsuarioAsync(int idUsuario);

        Task<CajaTurno> AbrirTurnoAsync(CajaTurno turno);
        Task<bool> CerrarTurnoAsync(CajaTurno turno);

        Task<(IReadOnlyList<CajaTurno> Items, int Total)> BuscarAsync(
            int? idCaja, int? idUsuario, DateTime? desde, DateTime? hasta,
            int page = 1, int pageSize = 10);
    }
}
