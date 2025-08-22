using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repo;
        private readonly IMapper _mapper;

        public EmpleadoService(IEmpleadoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EmpleadoDto?> ObtenerPorIdAsync(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return emp is null ? null : _mapper.Map<EmpleadoDto>(emp);
        }

        public async Task<PagedResponse<EmpleadoDto>> BuscarAsync(EmpleadoQueryParams query)
        {
            var (items, total) = await _repo.SearchAsync(query);
            var dtos = _mapper.Map<IReadOnlyList<EmpleadoDto>>(items);

            var page = query.page <= 0 ? 1 : query.page;
            var size = query.pageSize <= 0 ? 10 : query.pageSize;
            var totalPages = (int)Math.Ceiling(total / (double)size);

            return new PagedResponse<EmpleadoDto>
            {
                Items = dtos.ToList(),
                Total = total,
                Page = page,
                PageSize = size
            };
        }

        public async Task<EmpleadoDto> CrearAsync(CrearEmpleadoDto dto)
        {
            var entity = _mapper.Map<Empleado>(dto);
            // created/updated se establecen en el mapping (DateTime.UtcNow)
            await _repo.AddAsync(entity);

            // Recargar si quieres valores definitivos desde BD (por si hay triggers)
            var recargado = await _repo.GetByIdAsync(entity.Id_Empleado) ?? entity;
            return _mapper.Map<EmpleadoDto>(recargado);
        }

        public async Task<bool> ActualizarAsync(ActualizarEmpleadoDto dto)
        {
            var existente = await _repo.GetByIdAsync(dto.Id_Empleado);
            if (existente is null) return false;

            // Mapear cambios sobre la entidad
            _mapper.Map(dto, existente);
            await _repo.UpdateAsync(existente);
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var existente = await _repo.GetByIdAsync(id);
            if (existente is null) return false;

            // Si prefieres borrado lógico:
            // existente.Estatus = "Baja";
            // await _repo.UpdateAsync(existente);
            // return true;

            await _repo.DeleteAsync(existente);
            return true;
        }
    }
}
