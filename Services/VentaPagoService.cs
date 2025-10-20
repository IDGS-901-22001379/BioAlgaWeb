using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class VentaPagoService : IVentaPagoService
    {
        private static readonly HashSet<string> METODOS =
            new(StringComparer.OrdinalIgnoreCase) { "Efectivo", "Tarjeta", "Transferencia", "Otro" };

        private readonly IVentaPagoRepository _repo;
        private readonly IMapper _mapper;

        public VentaPagoService(IVentaPagoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<VentaPagoDto>> ListarPorVentaAsync(int idVenta)
        {
            var items = await _repo.GetByVentaAsync(idVenta);
            return _mapper.Map<IReadOnlyList<VentaPagoDto>>(items);
        }

        public async Task<VentaPagoDto> CrearAsync(CrearVentaPagoDto dto)
        {
            dto.Metodo = dto.Metodo?.Trim() ?? string.Empty;
            if (dto.Id_Venta <= 0) throw new ArgumentException("Id_Venta inválido.");
            if (!METODOS.Contains(dto.Metodo)) throw new ArgumentException("Método inválido.");
            if (dto.Monto <= 0) throw new ArgumentException("Monto debe ser mayor a 0.");

            var entity = _mapper.Map<VentaPago>(dto);
            var created = await _repo.AddAsync(entity);
            return _mapper.Map<VentaPagoDto>(created);
        }

        public async Task<bool> EliminarAsync(int idPago)
            => await _repo.DeleteAsync(idPago);

        public async Task<decimal> TotalPorMetodoAsync(int idVenta, string metodo)
        {
            metodo = metodo?.Trim() ?? string.Empty;
            if (!METODOS.Contains(metodo)) throw new ArgumentException("Método inválido.");
            return await _repo.TotalPorMetodoAsync(idVenta, metodo);
        }
    }
}
