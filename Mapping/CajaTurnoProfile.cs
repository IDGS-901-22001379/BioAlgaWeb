using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class CajaTurnoProfile : Profile
    {
        public CajaTurnoProfile()
        {
            // === Entity -> DTO básico (lectura/listado) ===
            CreateMap<CajaTurno, CajaTurnoDto>();

            // === Entity -> DTO enriquecido (opcional) ===
            // Ajustado a tu entidad Usuario con Nombre_Usuario
            CreateMap<CajaTurno, CajaTurnoResponseDto>()
                .IncludeBase<CajaTurno, CajaTurnoDto>()
                .ForMember(d => d.NombreCaja,
                    o => o.MapFrom(s => s.Caja != null ? s.Caja.Nombre : string.Empty))
                .ForMember(d => d.NombreUsuario,
                    o => o.MapFrom(s => s.Usuario != null ? s.Usuario.Nombre_Usuario : string.Empty));

            // ⚠️ NO mapear AbrirTurnoDto -> CajaTurno
            // (El Service crea la entidad y asigna IdCaja/IdUsuario/Apertura)

            // === CerrarTurnoDto -> Entity (actualización controlada) ===
            // Ignoramos explícitamente lo que NO debe tocarse.
            CreateMap<CerrarTurnoDto, CajaTurno>()
                // SOLO actualizamos estos:
                .ForMember(d => d.SaldoCierre, o => o.MapFrom(s => s.SaldoCierre))
                .ForMember(d => d.Observaciones, o =>
                {
                    o.PreCondition(src => !string.IsNullOrWhiteSpace(src.Observaciones));
                    o.MapFrom(src => src.Observaciones!.Trim());
                })
                // Ignorar TODO lo demás (para evitar cambios indeseados)
                .ForMember(d => d.IdTurno, o => o.Ignore())
                .ForMember(d => d.IdCaja, o => o.Ignore())
                .ForMember(d => d.IdUsuario, o => o.Ignore())
                .ForMember(d => d.Apertura, o => o.Ignore())
                .ForMember(d => d.Cierre, o => o.Ignore())
                .ForMember(d => d.SaldoInicial, o => o.Ignore());
        }
    }
}
