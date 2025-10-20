using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class VentaPagoProfile : Profile
    {
        public VentaPagoProfile()
        {
            // ===== Modelo -> DTO =====
            CreateMap<VentaPago, VentaPagoDto>()
                .ForMember(d => d.Id_Pago, o => o.MapFrom(s => s.IdPago))
                .ForMember(d => d.Id_Venta, o => o.MapFrom(s => s.IdVenta))
                .ForMember(d => d.Creado_En, o => o.MapFrom(s => s.CreadoEn));

            // ===== Crear DTO -> Modelo =====
            CreateMap<CrearVentaPagoDto, VentaPago>()
                .ForMember(d => d.IdPago, o => o.Ignore())
                .ForMember(d => d.IdVenta, o => o.MapFrom(s => s.Id_Venta))
                .ForMember(d => d.Metodo, o => o.MapFrom(s => s.Metodo))
                .ForMember(d => d.Monto, o => o.MapFrom(s => s.Monto))
                .ForMember(d => d.CreadoEn, o => o.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
