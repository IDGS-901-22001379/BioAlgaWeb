using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class CajaMovimientoProfile : Profile
    {
        public CajaMovimientoProfile()
        {
            // ===== Modelo -> DTO =====
            CreateMap<CajaMovimiento, CajaMovimientoDto>()
                .ForMember(d => d.Id_Mov, o => o.MapFrom(s => s.IdMov))
                .ForMember(d => d.Id_Turno, o => o.MapFrom(s => s.IdTurno))
                .ForMember(d => d.Monto, o => o.MapFrom(s => s.Monto));

            // ===== Crear DTO -> Modelo =====
            CreateMap<CrearCajaMovimientoDto, CajaMovimiento>()
                .ForMember(d => d.IdMov, o => o.Ignore())
                .ForMember(d => d.IdTurno, o => o.MapFrom(s => s.Id_Turno))
                .ForMember(d => d.Tipo, o => o.MapFrom(s => s.Tipo))          // 'Ingreso' | 'Egreso'
                .ForMember(d => d.Concepto, o => o.MapFrom(s => s.Concepto))
                .ForMember(d => d.Monto, o => o.MapFrom(s => s.Monto))
                .ForMember(d => d.Referencia, o => o.MapFrom(s => s.Referencia))
                .ForMember(d => d.Fecha, o => o.MapFrom(_ => DateTime.UtcNow));

            // ===== Actualizar DTO -> Modelo =====
            CreateMap<ActualizarCajaMovimientoDto, CajaMovimiento>()
                .ForMember(d => d.IdMov, o => o.Ignore())
                .ForMember(d => d.IdTurno, o => o.Ignore())   // no cambiar de turno
                .ForMember(d => d.Fecha, o => o.Ignore());  // fecha original se mantiene
        }
    }
}
