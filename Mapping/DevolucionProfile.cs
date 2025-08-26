using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping;

public class DevolucionProfile : Profile
{
    public DevolucionProfile()
    {
        // ===== Modelo -> DTO =====
        CreateMap<Devolucion, DevolucionDto>()
            .ForMember(d => d.IdDevolucion, o => o.MapFrom(s => s.IdDevolucion))
            .ForMember(d => d.IdVenta, o => o.MapFrom(s => s.IdVenta))
            .ForMember(d => d.Fecha, o => o.MapFrom(s => s.Fecha))
            .ForMember(d => d.Motivo, o => o.MapFrom(s => s.Motivo))
            .ForMember(d => d.ReingresaInventario, o => o.MapFrom(s => s.ReingresaInventario))
            .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.Subtotal))
            .ForMember(d => d.Impuestos, o => o.MapFrom(s => s.Impuestos))
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Total))
            .ForMember(d => d.Lineas, o => o.MapFrom(s =>
                s.Detalle.Select(l => new DevolucionLineaCreate
                {
                    IdProducto = l.IdProducto,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = l.PrecioUnitario,
                    IvaUnitario = l.IvaUnitario
                }).ToList()
            ));

        // ===== DTO Create -> Modelo =====
        CreateMap<DevolucionCreateRequest, Devolucion>()
            .ForMember(d => d.Detalle, o => o.Ignore()); // igual, se asigna después

        CreateMap<DevolucionLineaCreate, DetalleDevolucion>();
    }
}
