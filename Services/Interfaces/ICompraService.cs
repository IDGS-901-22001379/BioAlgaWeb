using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces;

public interface ICompraService
{
    Task<CompraDto> CrearBorradorAsync(CrearCompraDto dto);
    Task<CompraDto?> ObtenerAsync(int id);
    Task<PagedResponse<CompraDto>> BuscarAsync(CompraQueryParams p);
    Task<CompraDto?> AgregarRenglonAsync(int idCompra, AgregarRenglonDto dto);
    Task<CompraDto?> EliminarRenglonAsync(int idCompra, int idDetalle);
    Task<ConfirmarCompraResponse?> ConfirmarAsync(int idCompra, int idUsuario);
}
