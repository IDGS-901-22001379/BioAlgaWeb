using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<bool> ExistsUserNameAsync(string userName);
        Task AddAsync(Usuario usuario);
        void Update(Usuario usuario);
        void Remove(Usuario usuario);
        Task<(IReadOnlyList<Usuario> Items, int Total)> SearchAsync(UsuarioQueryParams q);
        Task<int> SaveChangesAsync();
    }
}
