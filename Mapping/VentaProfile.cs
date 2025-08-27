using System;
using System.Linq;
using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class VentaProfile : Profile
    {
        public VentaProfile()
        {
            // ===== Modelo -> DTO (como ya lo tenías) =====
            CreateMap<Venta, VentaDto>()
                .ForMember(d => d.Lineas, o => o.MapFrom(s =>
                    s.Detalle.Select(l => new VentaLineaCreate
                    {
                        IdProducto        = l.IdProducto,
                        Cantidad          = l.Cantidad,
                        PrecioUnitario    = l.PrecioUnitario,
                        DescuentoUnitario = l.DescuentoUnitario,
                        IvaUnitario       = l.IvaUnitario
                    }).ToList()
                ));

            // ===== DTO CREATE -> MODELO =====
            // Solo mapeamos lo que viene en el request.
            // Lo demás lo pones en el servicio (fechas, totales, estatus, usuario, detalle, etc.)
            CreateMap<VentaCreateRequest, Venta>()
                .ForMember(d => d.IdVenta,            o => o.Ignore())
                .ForMember(d => d.IdUsuario,          o => o.Ignore())   // lo pones desde el controller/servicio
                .ForMember(d => d.ClienteId,          o => o.MapFrom(s => s.ClienteId))
                .ForMember(d => d.MetodoPago,         o => o.MapFrom(s => s.MetodoPago))
                .ForMember(d => d.EfectivoRecibido,   o => o.MapFrom(s => s.EfectivoRecibido))

                // Calculados/establecidos en el servicio:
                .ForMember(d => d.Detalle,            o => o.Ignore())
                .ForMember(d => d.Subtotal,           o => o.Ignore())
                .ForMember(d => d.Impuestos,          o => o.Ignore())
                .ForMember(d => d.Total,              o => o.Ignore())
                .ForMember(d => d.Cambio,             o => o.Ignore())
                .ForMember(d => d.Estatus,            o => o.Ignore())
                .ForMember(d => d.FechaVenta,         o => o.Ignore());

            CreateMap<VentaLineaCreate, DetalleVenta>()
                .ForMember(d => d.IdDetalle,          o => o.Ignore())
                .ForMember(d => d.IdVenta,            o => o.Ignore());
        }
    }
}
