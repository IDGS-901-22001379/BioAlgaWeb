using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // === Hash SHA-256 (igual que en tu controller) ===
        private static string HashSha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u is null ? null : _mapper.Map<UsuarioDto>(u);
        }

        public async Task<(IReadOnlyList<UsuarioDto> Items, int Total)> BuscarAsync(UsuarioQueryParams q)
        {
            var (items, total) = await _repo.SearchAsync(q);
            return (_mapper.Map<IReadOnlyList<UsuarioDto>>(items), total);
        }

        public async Task<UsuarioDto> CrearAsync(UsuarioCreateRequest dto)
        {
            // Validación: nombre de usuario único
            if (await _repo.ExistsUserNameAsync(dto.Nombre_Usuario))
                throw new InvalidOperationException("El nombre de usuario ya existe.");

            // Mapear request -> entidad
            var entity = _mapper.Map<Usuario>(dto);
            entity.Contrasena = HashSha256(dto.Contrasena);
            entity.Fecha_Registro = DateTime.UtcNow;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Volver a leer con Includes para DTO completo
            var creado = await _repo.GetByIdAsync(entity.Id_Usuario)
                         ?? throw new InvalidOperationException("Error al crear el usuario.");

            return _mapper.Map<UsuarioDto>(creado);
        }

        public async Task<UsuarioDto> ActualizarAsync(int id, UsuarioUpdateRequest dto)
        {
            var user = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Usuario no encontrado.");

            // Si cambia el nombre de usuario, validar duplicado
            if (!string.IsNullOrWhiteSpace(dto.Nombre_Usuario) &&
                dto.Nombre_Usuario != user.Nombre_Usuario &&
                await _repo.ExistsUserNameAsync(dto.Nombre_Usuario))
            {
                throw new InvalidOperationException("El nombre de usuario ya está en uso.");
            }

            // Actualizar campos no nulos (configurado en el Profile)
            _mapper.Map(dto, user);

            // Si llega contraseña, re-hashear
            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
                user.Contrasena = HashSha256(dto.Contrasena);

            _repo.Update(user);
            await _repo.SaveChangesAsync();

            var actualizado = await _repo.GetByIdAsync(id)
                             ?? throw new InvalidOperationException("Error al actualizar el usuario.");

            return _mapper.Map<UsuarioDto>(actualizado);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user is null) return false;

            _repo.Remove(user);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
