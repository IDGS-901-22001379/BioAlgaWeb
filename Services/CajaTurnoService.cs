using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BioAlga.Backend.Services
{
    public class CajaTurnoService : ICajaTurnoService
    {
        private readonly ICajaTurnoRepository _repo;
        private readonly ICajaRepository _cajaRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CajaTurnoService> _logger;

        public CajaTurnoService(
            ICajaTurnoRepository repo,
            ICajaRepository cajaRepo,
            IUsuarioRepository usuarioRepo,
            IMapper mapper,
            ILogger<CajaTurnoService> logger)
        {
            _repo = repo;
            _cajaRepo = cajaRepo;
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResponse<CajaTurnoDto>> BuscarAsync(
            int? idCaja, int? idUsuario, DateTime? desde, DateTime? hasta, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = (pageSize <= 0 || pageSize > 200) ? 10 : pageSize;

            var (items, total) = await _repo.BuscarAsync(idCaja, idUsuario, desde, hasta, page, pageSize);
            var dtos = _mapper.Map<IReadOnlyList<CajaTurnoDto>>(items);

            return new PagedResponse<CajaTurnoDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = dtos
            };
        }

        public async Task<CajaTurnoDto?> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<CajaTurnoDto>(entity);
        }

        public async Task<CajaTurnoDto?> ObtenerTurnoAbiertoPorCajaAsync(int idCaja)
        {
            var entity = await _repo.GetTurnoAbiertoPorCajaAsync(idCaja);
            return entity is null ? null : _mapper.Map<CajaTurnoDto>(entity);
        }

        public async Task<CajaTurnoDto?> ObtenerTurnoAbiertoPorUsuarioAsync(int idUsuario)
        {
            var entity = await _repo.GetTurnoAbiertoPorUsuarioAsync(idUsuario);
            return entity is null ? null : _mapper.Map<CajaTurnoDto>(entity);
        }

        public async Task<CajaTurnoDto> AbrirTurnoAsync(AbrirTurnoDto dto)
        {
            // Validaciones del request
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                throw new ArgumentException("NombreUsuario es requerido.");
            if (string.IsNullOrWhiteSpace(dto.NombreCaja))
                throw new ArgumentException("NombreCaja es requerido.");
            if (dto.SaldoInicial < 0)
                throw new ArgumentException("SaldoInicial no puede ser negativo.");

            // Resolver usuario (solo activos)

            var usuario = await _usuarioRepo.GetByUserNameAsync(dto.NombreUsuario, soloActivos: true);

            if (usuario is null)
                throw new ArgumentException("Usuario no encontrado o inactivo.");

            // Resolver o crear caja
            var caja = await _cajaRepo.GetByNombreAsync(dto.NombreCaja);
            if (caja is null)
            {
                caja = new Caja
                {
                    Nombre = dto.NombreCaja.Trim(),
                    Descripcion = dto.DescripcionCaja?.Trim() ?? string.Empty
                };
                await _cajaRepo.AddAsync(caja); // debe persistir y devolver con IdCaja asignado
            }

            // Reglas: no permitir turno abierto por caja ni por usuario
            if (await _repo.GetTurnoAbiertoPorCajaAsync(caja.IdCaja) is not null)
                throw new InvalidOperationException("Ya existe un turno abierto para esa caja.");

            if (await _repo.GetTurnoAbiertoPorUsuarioAsync(usuario.Id_Usuario) is not null)
                throw new InvalidOperationException("Ese usuario ya tiene un turno abierto.");

            var entity = new CajaTurno
            {
                IdCaja = caja.IdCaja,
                IdUsuario = usuario.Id_Usuario, // <-- usa la propiedad real de tu entidad
                Apertura = DateTime.UtcNow,
                Cierre = null,
                SaldoInicial = dto.SaldoInicial,
                SaldoCierre = null,
                Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones!.Trim()
            };


            _logger.LogInformation("Abriendo turno: Caja={IdCaja}, Usuario={IdUsuario}, SaldoInicial={Saldo}",
                entity.IdCaja, entity.IdUsuario, entity.SaldoInicial);

            var created = await _repo.AbrirTurnoAsync(entity);
            return _mapper.Map<CajaTurnoDto>(created);
        }

        public async Task<CajaTurnoDto?> CerrarTurnoAsync(int idTurno, CerrarTurnoDto dto)
        {
            if (idTurno <= 0)
                throw new ArgumentException("IdTurno inválido.");
            if (dto.SaldoCierre < 0)
                throw new ArgumentException("SaldoCierre no puede ser negativo.");

            var current = await _repo.GetByIdAsync(idTurno);
            if (current is null) return null;
            if (current.Cierre != null)
                throw new InvalidOperationException("El turno ya está cerrado.");

            current.Cierre = DateTime.UtcNow;
            current.SaldoCierre = dto.SaldoCierre;
            if (!string.IsNullOrWhiteSpace(dto.Observaciones))
                current.Observaciones = dto.Observaciones!.Trim();

            var ok = await _repo.CerrarTurnoAsync(current);
            return ok ? _mapper.Map<CajaTurnoDto>(current) : null;
        }
    }
}
