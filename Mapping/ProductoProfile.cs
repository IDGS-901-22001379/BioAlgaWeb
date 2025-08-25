using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            CreateMap<Producto, ProductoDto>()
                .ForMember(d => d.Id_Producto, o => o.MapFrom(s => s.IdProducto))
                .ForMember(d => d.Codigo_Sku, o => o.MapFrom(s => s.CodigoSku))
                .ForMember(d => d.Codigo_Barras, o => o.MapFrom(s => s.CodigoBarras))
                .ForMember(d => d.Created_At, o => o.MapFrom(s => s.Created_At))
                .ForMember(d => d.Updated_At, o => o.MapFrom(s => s.Updated_At));

            CreateMap<CrearProductoDto, Producto>()
                .ForMember(d => d.IdProducto, o => o.Ignore())
                .ForMember(d => d.Created_At, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.Updated_At, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<ActualizarProductoDto, Producto>()
                .ForMember(d => d.Updated_At, o => o.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
