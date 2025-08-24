using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _repo;
        private readonly IMapper _mapper;

        public ProveedorService(IProveedorRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ProveedorDto?> ObtenerPorIdAsync(int id)
        {
            var ent = await _repo.GetByIdAsync(id);
            return ent is null ? null : _mapper.Map<ProveedorDto>(ent);
        }

        public async Task<PagedResponse<ProveedorDto>> BuscarAsync(ProveedorQueryParams p)
        {
            var q = _repo.Query();

            // Filtros
            if (!string.IsNullOrWhiteSpace(p.Q))
            {
                var ql = p.Q.Trim().ToLower();
                q = q.Where(x =>
                    x.NombreEmpresa.ToLower().Contains(ql) ||
                    (x.Contacto ?? "").ToLower().Contains(ql) ||
                    (x.Correo ?? "").ToLower().Contains(ql) ||
                    (x.Rfc ?? "").ToLower().Contains(ql) ||
                    (x.Telefono ?? "").ToLower().Contains(ql) ||
                    (x.Ciudad ?? "").ToLower().Contains(ql) ||
                    (x.Pais ?? "").ToLower().Contains(ql)
                );
            }
            if (!string.IsNullOrWhiteSpace(p.Estatus))
                q = q.Where(x => x.Estatus == p.Estatus);
            if (!string.IsNullOrWhiteSpace(p.Pais))
                q = q.Where(x => x.Pais == p.Pais);
            if (!string.IsNullOrWhiteSpace(p.Ciudad))
                q = q.Where(x => x.Ciudad == p.Ciudad);

            // Orden
            bool desc = (p.SortDir ?? "asc").Equals("desc", StringComparison.OrdinalIgnoreCase);
            q = (p.SortBy ?? "Nombre_Empresa") switch
            {
                "Pais"          => desc ? q.OrderByDescending(x => x.Pais)          : q.OrderBy(x => x.Pais),
                "Ciudad"        => desc ? q.OrderByDescending(x => x.Ciudad)        : q.OrderBy(x => x.Ciudad),
                "Estatus"       => desc ? q.OrderByDescending(x => x.Estatus)       : q.OrderBy(x => x.Estatus),
                "Created_At"    => desc ? q.OrderByDescending(x => x.CreatedAt)     : q.OrderBy(x => x.CreatedAt),
                _               => desc ? q.OrderByDescending(x => x.NombreEmpresa) : q.OrderBy(x => x.NombreEmpresa),
            };

            // Paginación
            int page = Math.Max(1, p.Page);
            int size = Math.Clamp(p.PageSize, 1, 100);
            int total = await q.CountAsync();
            var data = await q.Skip((page - 1) * size).Take(size)
                              .ProjectTo<ProveedorDto>(_mapper.ConfigurationProvider)
                              .ToListAsync();

            return new PagedResponse<ProveedorDto>
            {
                Items = data,
                Page = page,
                PageSize = size,
                Total = total
            };
        }

        public async Task<ProveedorDto> CrearAsync(CrearProveedorDto dto)
        {
            // Regla simple: no duplicar nombre exacto
            if (await _repo.ExistsByNombreAsync(dto.Nombre_Empresa))
                throw new InvalidOperationException("Ya existe un proveedor con ese nombre.");

            var ent = _mapper.Map<Proveedor>(dto);
            ent.Estatus = "Activo";
            var saved = await _repo.AddAsync(ent);
            return _mapper.Map<ProveedorDto>(saved);
        }

        public async Task<ProveedorDto?> ActualizarAsync(int id, ActualizarProveedorDto dto)
        {
            var ent = await _repo.GetByIdAsync(id);
            if (ent is null) return null;

            if (await _repo.ExistsByNombreAsync(dto.Nombre_Empresa, id))
                throw new InvalidOperationException("Ya existe otro proveedor con ese nombre.");

            _mapper.Map(dto, ent);
            await _repo.UpdateAsync(ent);
            return _mapper.Map<ProveedorDto>(ent);
        }

        public async Task<bool> CambiarEstatusAsync(int id, string nuevoEstatus)
        {
            var ent = await _repo.GetByIdAsync(id);
            if (ent is null) return false;

            ent.Estatus = (nuevoEstatus == "Inactivo") ? "Inactivo" : "Activo";
            await _repo.UpdateAsync(ent);
            return true;
        }

        public async Task<bool> EliminarDuroAsync(int id)
        {
            var ent = await _repo.GetByIdAsync(id);
            if (ent is null) return false;
            await _repo.DeleteAsync(ent);
            return true;
        }
    }
}
