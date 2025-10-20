using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class CajaTurnoProfile : Profile
    {
        public CajaTurnoProfile()
        {
            // ===== Modelo -> DTO =====
            CreateMap<CajaTurno, CajaTurnoDto>()
                .ForMember(d => d.Id_Turno, o => o.MapFrom(s => s.IdTurno))
                .ForMember(d => d.Id_Caja, o => o.MapFrom(s => s.IdCaja))
                .ForMember(d => d.Id_Usuario, o => o.MapFrom(s => s.IdUsuario))
                .ForMember(d => d.Saldo_Inicial, o => o.MapFrom(s => s.SaldoInicial))
                .ForMember(d => d.Saldo_Cierre, o => o.MapFrom(s => s.SaldoCierre));

            // ===== Abrir turno (Crear) =====
            CreateMap<AbrirTurnoDto, CajaTurno>()
                .ForMember(d => d.IdTurno, o => o.Ignore())
                .ForMember(d => d.Apertura, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.Cierre, o => o.Ignore())
                .ForMember(d => d.SaldoInicial, o => o.MapFrom(s => s.Saldo_Inicial))
                .ForMember(d => d.SaldoCierre, o => o.Ignore());

            // ===== Cerrar turno (Actualizar parcial) =====
            CreateMap<CerrarTurnoDto, CajaTurno>()
                .ForMember(d => d.IdTurno, o => o.Ignore())
                .ForMember(d => d.Cierre, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.SaldoCierre, o => o.MapFrom(s => s.Saldo_Cierre))
                .ForMember(d => d.SaldoInicial, o => o.Ignore())   // no se toca
                .ForMember(d => d.IdCaja, o => o.Ignore())
                .ForMember(d => d.IdUsuario, o => o.Ignore())
                .ForMember(d => d.Apertura, o => o.Ignore());
        }
    }
}
