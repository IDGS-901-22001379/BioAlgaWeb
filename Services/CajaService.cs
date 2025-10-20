using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _repo;
        private readonly IMapper _mapper;

        public CajaService(ICajaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<CajaDto>> ListarAsync()
        {
            var items = await _repo.GetAllAsync();
            return _mapper.Map<IReadOnlyList<CajaDto>>(items);
        }

        public async Task<CajaDto?> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<CajaDto>(entity);
        }

        public async Task<CajaDto> CrearAsync(CrearCajaDto dto)
        {
            dto.Nombre = dto.Nombre?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la caja es obligatorio.");

            var exists = await _repo.NombreExistsAsync(dto.Nombre);
            if (exists) throw new InvalidOperationException("Ya existe una caja con ese nombre.");

            var entity = _mapper.Map<Caja>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<CajaDto>(created);
        }

        public async Task<CajaDto?> ActualizarAsync(int id, ActualizarCajaDto dto)
        {
            dto.Nombre = dto.Nombre?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la caja es obligatorio.");

            var current = await _repo.GetByIdAsync(id);
            if (current is null) return null;

            var exists = await _repo.NombreExistsAsync(dto.Nombre, excludeId: id);
            if (exists) throw new InvalidOperationException("Ya existe otra caja con ese nombre.");

            current.Nombre = dto.Nombre;
            current.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();

            var ok = await _repo.UpdateAsync(current);
            return ok ? _mapper.Map<CajaDto>(current) : null;
        }

        public async Task<bool> EliminarAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}
