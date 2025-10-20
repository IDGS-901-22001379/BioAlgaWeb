using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class CajaProfile : Profile
    {
        public CajaProfile()
        {
            // ===== Modelo -> DTO =====
            CreateMap<Caja, CajaDto>()
                .ForMember(d => d.Id_Caja, o => o.MapFrom(s => s.IdCaja))
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Descripcion));

            // ===== Crear DTO -> Modelo =====
            CreateMap<CrearCajaDto, Caja>()
                .ForMember(d => d.IdCaja, o => o.Ignore());

            // ===== Actualizar DTO -> Modelo =====
            CreateMap<ActualizarCajaDto, Caja>()
                .ForMember(d => d.IdCaja, o => o.Ignore());
        }
    }
}
