using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface ICajaMovimientoService
    {
        Task<PagedResponse<CajaMovimientoDto>> BuscarPorTurnoAsync(
            int idTurno, string? tipo, string? qConcepto, int page, int pageSize);

        Task<CajaMovimientoDto?> ObtenerPorIdAsync(int id);
        Task<CajaMovimientoDto> CrearAsync(CrearCajaMovimientoDto dto);
        Task<CajaMovimientoDto?> ActualizarAsync(int id, ActualizarCajaMovimientoDto dto);
        Task<bool> EliminarAsync(int id);

        // Totales rápidos para tarjeta “Dinero en Caja”
        Task<(decimal Entradas, decimal Salidas)> TotalesPorTurnoAsync(int idTurno);
    }
}
