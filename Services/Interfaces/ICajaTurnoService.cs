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

        Task<CajaTurnoDto> AbrirTurnoAsync(AbrirTurnoDto dto);
        Task<CajaTurnoDto?> CerrarTurnoAsync(int idTurno, CerrarTurnoDto dto);
    }
}
