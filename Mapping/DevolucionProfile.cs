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
            // Ejemplo de Profile
            CreateMap<Devolucion, DevolucionDto>()
                .ForMember(d => d.FechaDevolucion, m => m.MapFrom(s => s.FechaDevolucion))
                .ForMember(d => d.ReferenciaVenta, m => m.MapFrom(s => s.ReferenciaVenta));

            CreateMap<DetalleDevolucion, DevolucionDetalleDto>();

            CreateMap<DevolucionCreateRequest, Devolucion>()
                .ForMember(d => d.IdDevolucion, m => m.Ignore())
                .ForMember(d => d.FechaDevolucion, m => m.Ignore()) // la seteamos en el service
                .ForMember(d => d.IdUsuario, m => m.Ignore())
                .ForMember(d => d.UsuarioNombre, m => m.Ignore())
                .ForMember(d => d.TotalDevuelto, m => m.Ignore())
                .ForMember(d => d.ReferenciaVenta, m => m.MapFrom(s => s.ReferenciaVenta))
                .ForMember(d => d.Detalles, m => m.MapFrom(s => s.Lineas));

            CreateMap<DevolucionLineaCreate, DetalleDevolucion>()
                .ForMember(d => d.IdDetalle, m => m.Ignore())
                .ForMember(d => d.ProductoNombre, m => m.Ignore()); // lo llenamos en el service

        }
    }
}
