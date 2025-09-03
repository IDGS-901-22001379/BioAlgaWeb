using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoDto?> GetAsync(int idPedido);
        Task<PagedResponse<PedidoListItemDto>> BuscarAsync(
            string? q, EstatusPedido? estatus, int page, int pageSize,
            string? sortBy, string? sortDir);

        Task<PedidoDto> CrearAsync(int idUsuario, PedidoCreateRequest req);
        Task<PedidoDto> UpdateHeaderAsync(int idUsuario, PedidoUpdateHeaderRequest req);
        Task<PedidoDto> ReplaceLinesAsync(int idUsuario, PedidoReplaceLinesRequest req);
        Task<PedidoDto> EditLineAsync(int idUsuario, PedidoLineaEditRequest req);
        Task<PedidoDto> ConfirmarAsync(int idUsuario, PedidoConfirmarRequest req);
        Task<PedidoDto> CambiarEstatusAsync(int idUsuario, PedidoCambioEstatusRequest req);

        Task EliminarAsync(int idPedido); // permitido: Borrador / Cancelado
    }
}
