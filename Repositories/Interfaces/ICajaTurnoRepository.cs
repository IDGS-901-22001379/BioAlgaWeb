using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface ICajaTurnoRepository
    {
        Task<CajaTurno?> GetByIdAsync(int id);

        // Turno abierto (sin cierre) por caja/usuario
        Task<CajaTurno?> GetTurnoAbiertoPorCajaAsync(int idCaja);
        Task<CajaTurno?> GetTurnoAbiertoPorUsuarioAsync(int idUsuario);

        // Abrir/Cerrar
        Task<CajaTurno> AbrirTurnoAsync(CajaTurno turno);
        Task<bool> CerrarTurnoAsync(CajaTurno turno);

        // Búsqueda/paginación básica
        Task<(IReadOnlyList<CajaTurno> Items, int Total)> BuscarAsync(
            int? idCaja, int? idUsuario, DateTime? desde, DateTime? hasta,
            int page = 1, int pageSize = 10);
    }
}
