using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class PrecioProfile : Profile
    {
        public PrecioProfile()
        {
            CreateMap<ProductoPrecio, PrecioDto>()
                .ForMember(d => d.Id_Precio, o => o.MapFrom(s => s.IdPrecio))
                .ForMember(d => d.Id_Producto, o => o.MapFrom(s => s.IdProducto))
                .ForMember(d => d.Tipo_Precio, o => o.MapFrom(s => s.TipoPrecio))
                .ForMember(d => d.Vigente_Desde, o => o.MapFrom(s => s.VigenteDesde))
                .ForMember(d => d.Vigente_Hasta, o => o.MapFrom(s => s.VigenteHasta));

            CreateMap<CrearPrecioDto, ProductoPrecio>()
                .ForMember(d => d.IdPrecio, o => o.Ignore())
                .ForMember(d => d.VigenteDesde, o => o.MapFrom(s => s.VigenteDesde ?? DateTime.UtcNow));
        }
    }
}
