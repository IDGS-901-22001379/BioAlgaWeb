using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class DevolucionProfile : Profile
    {
        public DevolucionProfile()
        {
            // ===== Entity -> DTO =====
            CreateMap<DetalleDevolucion, DevolucionDetalleDto>();

            CreateMap<Devolucion, DevolucionDto>()
                .ForMember(d => d.NumeroLineas, o => o.MapFrom(s => s.Detalles.Count))
                .ForMember(d => d.Detalles, o => o.MapFrom(s => s.Detalles));

            // ===== CreateRequest -> Entity =====
            CreateMap<DevolucionLineaCreate, DetalleDevolucion>()
                .ForMember(d => d.IdDetalle, o => o.Ignore())
                .ForMember(d => d.ProductoNombre, o => o.Ignore()) // se llena en Service con snapshot del producto
                .ForMember(d => d.IdDevolucion, o => o.Ignore());  // lo asigna EF

            CreateMap<DevolucionCreateRequest, Devolucion>()
                .ForMember(d => d.IdDevolucion, o => o.Ignore())
                .ForMember(d => d.FechaDevolucion, o => o.Ignore())  // se deja default SQL/UTC
                .ForMember(d => d.IdUsuario, o => o.Ignore())        // se toma del contexto/autenticación
                .ForMember(d => d.UsuarioNombre, o => o.Ignore())    // lo arma el Service con nombre del usuario
                .ForMember(d => d.TotalDevuelto, o => o.Ignore())    // suma de líneas en Service
                .ForMember(d => d.Detalles, o => o.MapFrom(s => s.Lineas));
        }
    }
}
