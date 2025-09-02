using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IPedidoService
    {
        // Lectura
        Task<PedidoDto?> GetAsync(int idPedido);
        Task<PagedResponse<PedidoListItemDto>> BuscarAsync(
            string? q, EstatusPedido? estatus, int page = 1, int pageSize = 10,
            string? sortBy = "FechaPedido", string? sortDir = "DESC");

        // Escritura
        Task<PedidoDto> CrearAsync(int idUsuario, PedidoCreateRequest req);
        Task<PedidoDto> UpdateHeaderAsync(int idUsuario, PedidoUpdateHeaderRequest req);
        Task<PedidoDto> ReplaceLinesAsync(int idUsuario, PedidoReplaceLinesRequest req);
        Task<PedidoDto> EditLineAsync(int idUsuario, PedidoLineaEditRequest req);

        // Flujo
        Task<PedidoDto> ConfirmarAsync(int idUsuario, PedidoConfirmarRequest req);
        Task<PedidoDto> CambiarEstatusAsync(int idUsuario, PedidoCambioEstatusRequest req);

        // Interfaces/IPedidoService.cs
        Task EliminarAsync(int idPedido);

    }
}
