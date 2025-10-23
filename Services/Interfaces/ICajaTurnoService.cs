using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface ICajaTurnoService
    {
        Task<PagedResponse<CajaTurnoDto>> BuscarAsync(
            int? idCaja, int? idUsuario, DateTime? desde, DateTime? hasta, int page, int pageSize);

        Task<CajaTurnoDto?> ObtenerPorIdAsync(int id);
        Task<CajaTurnoDto?> ObtenerTurnoAbiertoPorCajaAsync(int idCaja);
        Task<CajaTurnoDto?> ObtenerTurnoAbiertoPorUsuarioAsync(int idUsuario);

        // Request sin IDs; service resuelve y devuelve DTO
        Task<CajaTurnoDto> AbrirTurnoAsync(AbrirTurnoDto dto);

        // Cierre por IdTurno (en ruta) + body con saldo y observaciones
        Task<CajaTurnoDto?> CerrarTurnoAsync(int idTurno, CerrarTurnoDto dto);
    }
}
