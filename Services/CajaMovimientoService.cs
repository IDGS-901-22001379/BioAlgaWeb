using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class CajaMovimientoService : ICajaMovimientoService
    {
        private static readonly HashSet<string> TIPOS =
            new(StringComparer.OrdinalIgnoreCase) { "Ingreso", "Egreso" };

        private readonly ICajaMovimientoRepository _repo;
        private readonly ICajaTurnoRepository _turnoRepo;
        private readonly IMapper _mapper;

        public CajaMovimientoService(ICajaMovimientoRepository repo, ICajaTurnoRepository turnoRepo, IMapper mapper)
        {
            _repo = repo;
            _turnoRepo = turnoRepo;
            _mapper = mapper;
        }

        public async Task<PagedResponse<CajaMovimientoDto>> BuscarPorTurnoAsync(
            int idTurno, string? tipo, string? qConcepto, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = (pageSize <= 0 || pageSize > 200) ? 20 : pageSize;

            tipo = string.IsNullOrWhiteSpace(tipo) ? null : tipo.Trim();
            if (tipo != null && !TIPOS.Contains(tipo))
                throw new ArgumentException("Tipo inválido. Usa: Ingreso o Egreso.");

            var (items, total) = await _repo.BuscarPorTurnoAsync(idTurno, tipo, qConcepto, page, pageSize);
            var dtos = _mapper.Map<IReadOnlyList<CajaMovimientoDto>>(items);

            return new PagedResponse<CajaMovimientoDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = dtos
            };
        }

        public async Task<CajaMovimientoDto?> ObtenerPorIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<CajaMovimientoDto>(entity);
        }

        public async Task<CajaMovimientoDto> CrearAsync(CrearCajaMovimientoDto dto)
        {
            Limpiar(dto);
            Validar(dto);

            var turno = await _turnoRepo.GetByIdAsync(dto.Id_Turno);
            if (turno is null) throw new ArgumentException("El turno no existe.");
            if (turno.Cierre != null) throw new InvalidOperationException("No se pueden registrar movimientos en un turno cerrado.");

            var entity = _mapper.Map<CajaMovimiento>(dto);
            entity.Fecha = DateTime.UtcNow;

            var created = await _repo.AddAsync(entity);
            return _mapper.Map<CajaMovimientoDto>(created);
        }

        public async Task<CajaMovimientoDto?> ActualizarAsync(int id, ActualizarCajaMovimientoDto dto)
        {
            Limpiar(dto);
            Validar(dto);

            var current = await _repo.GetByIdAsync(id);
            if (current is null) return null;

            var turno = await _turnoRepo.GetByIdAsync(current.IdTurno);
            if (turno is null) throw new ArgumentException("El turno del movimiento no existe.");
            if (turno.Cierre != null) throw new InvalidOperationException("No se pueden modificar movimientos de un turno cerrado.");

            current.Tipo = dto.Tipo;
            current.Concepto = dto.Concepto!;
            current.Monto = dto.Monto;
            current.Referencia = dto.Referencia;

            var ok = await _repo.UpdateAsync(current);
            return ok ? _mapper.Map<CajaMovimientoDto>(current) : null;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current is null) return false;

            var turno = await _turnoRepo.GetByIdAsync(current.IdTurno);
            if (turno?.Cierre != null)
                throw new InvalidOperationException("No se pueden borrar movimientos de un turno cerrado.");

            return await _repo.DeleteAsync(id);
        }

        public async Task<(decimal Entradas, decimal Salidas)> TotalesPorTurnoAsync(int idTurno)
        {
            var (items, _) = await _repo.BuscarPorTurnoAsync(idTurno, null, null, 1, int.MaxValue);
            var entradas = items.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
            var salidas = items.Where(m => m.Tipo == "Egreso").Sum(m => m.Monto);
            return (entradas, salidas);
        }

        // Helpers
        private static void Limpiar(CrearCajaMovimientoDto dto)
        {
            dto.Tipo = dto.Tipo?.Trim() ?? string.Empty;
            dto.Concepto = dto.Concepto?.Trim() ?? string.Empty;
            dto.Referencia = string.IsNullOrWhiteSpace(dto.Referencia) ? null : dto.Referencia.Trim();
        }
        private static void Limpiar(ActualizarCajaMovimientoDto dto)
        {
            dto.Tipo = dto.Tipo?.Trim() ?? string.Empty;
            dto.Concepto = dto.Concepto?.Trim() ?? string.Empty;
            dto.Referencia = string.IsNullOrWhiteSpace(dto.Referencia) ? null : dto.Referencia.Trim();
        }
        private static void Validar(CrearCajaMovimientoDto dto)
        {
            if (dto.Id_Turno <= 0) throw new ArgumentException("Id_Turno inválido.");
            if (!TIPOS.Contains(dto.Tipo)) throw new ArgumentException("Tipo inválido. Usa: Ingreso o Egreso.");
            if (string.IsNullOrWhiteSpace(dto.Concepto)) throw new ArgumentException("Concepto es obligatorio.");
            if (dto.Monto <= 0) throw new ArgumentException("Monto debe ser mayor a 0.");
        }
        private static void Validar(ActualizarCajaMovimientoDto dto)
        {
            if (!TIPOS.Contains(dto.Tipo)) throw new ArgumentException("Tipo inválido. Usa: Ingreso o Egreso.");
            if (string.IsNullOrWhiteSpace(dto.Concepto)) throw new ArgumentException("Concepto es obligatorio.");
            if (dto.Monto <= 0) throw new ArgumentException("Monto debe ser mayor a 0.");
        }
    }
}
