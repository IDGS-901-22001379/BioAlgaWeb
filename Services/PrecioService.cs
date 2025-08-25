using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class PrecioService : IPrecioService
    {
        private readonly IPrecioRepository _repo;
        private readonly IMapper _mapper;

        public PrecioService(IPrecioRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<PrecioDto>> GetHistorialAsync(int idProducto, CancellationToken ct = default)
        {
            var list = await _repo.GetHistorialAsync(idProducto, ct);
            return _mapper.Map<IReadOnlyList<PrecioDto>>(list);
        }

        public async Task<IReadOnlyList<PrecioDto>> GetVigentesAsync(int idProducto, CancellationToken ct = default)
        {
            var list = await _repo.GetVigentesAsync(idProducto, ct);
            return _mapper.Map<IReadOnlyList<PrecioDto>>(list);
        }

        public async Task<PrecioDto?> CrearAsync(int idProducto, CrearPrecioDto dto, CancellationToken ct = default)
        {
            // regla: desactivar vigente anterior del mismo tipo
            await _repo.DesactivarVigenteDelMismoTipoAsync(idProducto, dto.TipoPrecio, dto.VigenteDesde, ct);

            var entity = _mapper.Map<ProductoPrecio>(dto);
            entity.IdProducto = idProducto;

            var creado = await _repo.AddAsync(entity, ct);
            return _mapper.Map<PrecioDto>(creado);
        }

        public async Task<bool> ActualizarAsync(int idPrecio, ActualizarPrecioDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(idPrecio, ct);
            if (entity is null) return false;

            if (dto.Precio != default) entity.Precio = dto.Precio;
            if (dto.VigenteHasta.HasValue) entity.VigenteHasta = dto.VigenteHasta;
            if (dto.Activo.HasValue) entity.Activo = dto.Activo.Value;

            await _repo.UpdateAsync(entity, ct);
            return true;
        }
    }
}
