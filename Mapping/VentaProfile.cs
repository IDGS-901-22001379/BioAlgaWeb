using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping;

public class VentaProfile : Profile
{
    public VentaProfile()
    {
        // ===== Modelo -> DTO =====
        CreateMap<Venta, VentaDto>()
            .ForMember(d => d.IdVenta, o => o.MapFrom(s => s.IdVenta))
            .ForMember(d => d.ClienteId, o => o.MapFrom(s => s.ClienteId))
            .ForMember(d => d.FechaVenta, o => o.MapFrom(s => s.FechaVenta))
            .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.Subtotal))
            .ForMember(d => d.Impuestos, o => o.MapFrom(s => s.Impuestos))
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Total))
            .ForMember(d => d.MetodoPago, o => o.MapFrom(s => s.MetodoPago))
            .ForMember(d => d.EfectivoRecibido, o => o.MapFrom(s => s.EfectivoRecibido))
            .ForMember(d => d.Cambio, o => o.MapFrom(s => s.Cambio))
            .ForMember(d => d.Estatus, o => o.MapFrom(s => s.Estatus.ToString()))
            // Lineas del detalle -> reutilizamos VentaLineaCreate
            .ForMember(d => d.Lineas, o => o.MapFrom(s =>
                s.Detalle.Select(l => new VentaLineaCreate
                {
                    IdProducto = l.IdProducto,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = l.PrecioUnitario,
                    DescuentoUnitario = l.DescuentoUnitario,
                    IvaUnitario = l.IvaUnitario
                }).ToList()
            ));

        // ===== DTO Create -> Modelo =====
        CreateMap<VentaCreateRequest, Venta>()
            .ForMember(d => d.Detalle, o => o.Ignore()); // se carga manualmente

        CreateMap<VentaLineaCreate, DetalleVenta>();
    }
}
