using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Models.Enums;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            // ====== Pedido → DTOs de lectura ======
            CreateMap<Pedido, PedidoListItemDto>()
                .ForMember(dest => dest.ClienteNombre,
                    opt => opt.MapFrom(src =>
                        src.Cliente != null
                            ? $"{src.Cliente.Nombre} {src.Cliente.ApellidoPaterno} {src.Cliente.ApellidoMaterno}"
                            : "Cliente desconocido"
                    ));

            CreateMap<Pedido, PedidoDto>()
                .ForMember(dest => dest.ClienteNombre,
                    opt => opt.MapFrom(src =>
                        src.Cliente != null
                            ? $"{src.Cliente.Nombre} {src.Cliente.ApellidoPaterno} {src.Cliente.ApellidoMaterno}"
                            : "Cliente desconocido"
                    ));

            CreateMap<DetallePedido, PedidoLineaDto>()
                .ForMember(dest => dest.ProductoNombre,
                    opt => opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : "Producto desconocido"));

            // ====== DTOs de creación → Pedido ======
            CreateMap<PedidoCreateRequest, Pedido>()
                .ForMember(dest => dest.Estatus, opt => opt.MapFrom(_ => EstatusPedido.Borrador))
                .ForMember(dest => dest.Detalles, opt => opt.Ignore()); // se cargan manualmente

            CreateMap<PedidoLineaCreateRequest, DetallePedido>()
                .ForMember(dest => dest.PrecioUnitario, opt => opt.Ignore()); 
                // el Precio se congela en Confirmación

            // ====== Update Header ======
            CreateMap<PedidoUpdateHeaderRequest, Pedido>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ====== Replace/Editar líneas ======
            CreateMap<PedidoLineaEditRequest, DetallePedido>()
                .ForMember(dest => dest.PrecioUnitario, opt => opt.Ignore());
        }
    }
}
