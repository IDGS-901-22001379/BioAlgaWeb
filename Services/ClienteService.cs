using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class ClienteService : IClienteService
    {
        private static readonly HashSet<string> TIPOS_PERMITIDOS =
            new(StringComparer.OrdinalIgnoreCase) { "Normal", "Mayoreo", "Especial", "Descuento" };

        private static readonly HashSet<string> ESTADOS_PERMITIDOS =
            new(StringComparer.OrdinalIgnoreCase) { "Activo", "Inactivo" };

        private readonly IClienteRepository _repo;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // ================== Buscar ==================
        public async Task<PagedResponse<ClienteDto>> BuscarAsync(ClienteQueryParams query)
        {
            NormalizarQuery(query);

            var (items, total) = await _repo.BuscarAsync(query);
            var dtos = _mapper.Map<IReadOnlyList<ClienteDto>>(items);

            return new PagedResponse<ClienteDto>
            {
                Page = query.Page <= 0 ? 1 : query.Page,
                PageSize = query.PageSize <= 0 ? 10 : query.PageSize,
                Total = total,
                Items = dtos
            };
        }

        // ================== Obtener ==================
        public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<ClienteDto>(entity);
        }

        // ================== Crear ==================
        public async Task<ClienteDto> CrearAsync(CrearClienteDto dto)
        {
            Limpiar(dto);
            ValidarDominio(dto);

            if (!string.IsNullOrWhiteSpace(dto.Correo))
            {
                var exists = await _repo.EmailExistsAsync(dto.Correo!);
                if (exists) throw new InvalidOperationException("El correo ya está registrado para otro cliente.");
            }

            var entity = _mapper.Map<Cliente>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<ClienteDto>(created);
        }

        // ================== Actualizar ==================
        public async Task<ClienteDto?> ActualizarAsync(int id, ActualizarClienteDto dto)
        {
            Limpiar(dto);
            ValidarDominio(dto);

            var current = await _repo.GetByIdAsync(id);
            if (current is null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Correo))
            {
                var exists = await _repo.EmailExistsAsync(dto.Correo!, excludeId: id);
                if (exists) throw new InvalidOperationException("El correo ya está registrado para otro cliente.");
            }

            // Mapear campos editables
            current.Nombre          = dto.Nombre;
            current.ApellidoPaterno = dto.Apellido_Paterno;
            current.ApellidoMaterno = dto.Apellido_Materno;
            current.Correo          = dto.Correo;
            current.Telefono        = dto.Telefono;
            current.Direccion       = dto.Direccion;
            current.TipoCliente     = dto.Tipo_Cliente;
            current.Estado          = dto.Estado;

            var ok = await _repo.UpdateAsync(current);
            return ok ? _mapper.Map<ClienteDto>(current) : null;
        }

        // ================== Eliminar ==================
        public async Task<bool> EliminarAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        // ================== Helpers ==================
        private static void Limpiar(CrearClienteDto dto)
        {
            dto.Nombre           = dto.Nombre?.Trim() ?? string.Empty;
            dto.Apellido_Paterno = dto.Apellido_Paterno?.Trim();
            dto.Apellido_Materno = dto.Apellido_Materno?.Trim();
            dto.Correo           = dto.Correo?.Trim();
            dto.Telefono         = dto.Telefono?.Trim();
            dto.Direccion        = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();
            dto.Tipo_Cliente     = string.IsNullOrWhiteSpace(dto.Tipo_Cliente) ? "Normal" : dto.Tipo_Cliente.Trim();
            dto.Estado           = string.IsNullOrWhiteSpace(dto.Estado) ? "Activo" : dto.Estado.Trim();
        }

        private static void Limpiar(ActualizarClienteDto dto)
        {
            dto.Nombre           = dto.Nombre?.Trim() ?? string.Empty;
            dto.Apellido_Paterno = dto.Apellido_Paterno?.Trim();
            dto.Apellido_Materno = dto.Apellido_Materno?.Trim();
            dto.Correo           = dto.Correo?.Trim();
            dto.Telefono         = dto.Telefono?.Trim();
            dto.Direccion        = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();
            dto.Tipo_Cliente     = string.IsNullOrWhiteSpace(dto.Tipo_Cliente) ? "Normal" : dto.Tipo_Cliente.Trim();
            dto.Estado           = string.IsNullOrWhiteSpace(dto.Estado) ? "Activo" : dto.Estado.Trim();
        }

        private static void ValidarDominio(CrearClienteDto dto)
        {
            if (!TIPOS_PERMITIDOS.Contains(dto.Tipo_Cliente))
                throw new ArgumentException("Tipo_Cliente inválido. Usa: Normal, Mayoreo, Especial o Descuento.");

            if (!ESTADOS_PERMITIDOS.Contains(dto.Estado))
                throw new ArgumentException("Estado inválido. Usa: Activo o Inactivo.");
        }

        private static void ValidarDominio(ActualizarClienteDto dto)
        {
            if (!TIPOS_PERMITIDOS.Contains(dto.Tipo_Cliente))
                throw new ArgumentException("Tipo_Cliente inválido. Usa: Normal, Mayoreo, Especial o Descuento.");

            if (!ESTADOS_PERMITIDOS.Contains(dto.Estado))
                throw new ArgumentException("Estado inválido. Usa: Activo o Inactivo.");
        }

        private static void NormalizarQuery(ClienteQueryParams q)
        {
            q.Q = string.IsNullOrWhiteSpace(q.Q) ? null : q.Q.Trim();
            q.Tipo_Cliente = string.IsNullOrWhiteSpace(q.Tipo_Cliente) ? null : q.Tipo_Cliente.Trim();
            q.Estado = string.IsNullOrWhiteSpace(q.Estado) ? null : q.Estado.Trim();

            if (!string.IsNullOrWhiteSpace(q.SortBy))
            {
                var sb = q.SortBy.ToLowerInvariant();
                q.SortBy = (sb == "fecha" || sb == "nombre") ? sb : "nombre";
            }

            if (!string.IsNullOrWhiteSpace(q.SortDir))
            {
                var sd = q.SortDir.ToLowerInvariant();
                q.SortDir = (sd == "asc" || sd == "desc") ? sd : "asc";
            }

            if (q.Page <= 0) q.Page = 1;
            if (q.PageSize <= 0 || q.PageSize > 200) q.PageSize = 10;
        }
    }
}
