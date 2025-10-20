using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class CajaTurnoService : ICajaTurnoService
    {
        private readonly ICajaTurnoRepository _repo;
        private readonly ICajaRepository _cajaRepo;
        private readonly IMapper _mapper;

        public CajaTurnoService(ICajaTurnoRepository repo, ICajaRepository cajaRepo, IMapper mapper)
        {
            _repo = repo;
            _cajaRepo = cajaRepo;
            _mapper = mapper;
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
            if (dto.Id_Caja <= 0) throw new ArgumentException("Id_Caja inválido.");
            if (dto.Id_Usuario <= 0) throw new ArgumentException("Id_Usuario inválido.");
            if (dto.Saldo_Inicial < 0) throw new ArgumentException("Saldo_Inicial no puede ser negativo.");

            var caja = await _cajaRepo.GetByIdAsync(dto.Id_Caja);
            if (caja is null) throw new ArgumentException("La caja no existe.");

            var abiertoCaja = await _repo.GetTurnoAbiertoPorCajaAsync(dto.Id_Caja);
            if (abiertoCaja is not null)
                throw new InvalidOperationException("Ya existe un turno abierto para esa caja.");

            var abiertoUsuario = await _repo.GetTurnoAbiertoPorUsuarioAsync(dto.Id_Usuario);
            if (abiertoUsuario is not null)
                throw new InvalidOperationException("Ese usuario ya tiene un turno abierto.");

            var entity = _mapper.Map<CajaTurno>(dto);
            entity.Apertura = DateTime.UtcNow;

            var created = await _repo.AbrirTurnoAsync(entity);
            return _mapper.Map<CajaTurnoDto>(created);
        }

        public async Task<CajaTurnoDto?> CerrarTurnoAsync(int idTurno, CerrarTurnoDto dto)
        {
            if (dto.Saldo_Cierre < 0) throw new ArgumentException("Saldo_Cierre no puede ser negativo.");

            var current = await _repo.GetByIdAsync(idTurno);
            if (current is null) return null;
            if (current.Cierre != null) throw new InvalidOperationException("El turno ya está cerrado.");

            current.Cierre = DateTime.UtcNow;
            current.SaldoCierre = dto.Saldo_Cierre;
            current.Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? current.Observaciones : dto.Observaciones!.Trim();

            var ok = await _repo.CerrarTurnoAsync(current);
            return ok ? _mapper.Map<CajaTurnoDto>(current) : null;
        }
    }
}
