using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<(IReadOnlyList<UsuarioDto> Items, int Total)> BuscarAsync(UsuarioQueryParams q);
        Task<UsuarioDto> CrearAsync(UsuarioCreateRequest dto);
        Task<UsuarioDto> ActualizarAsync(int id, UsuarioUpdateRequest dto);
        Task<bool> EliminarAsync(int id);
    }
}
