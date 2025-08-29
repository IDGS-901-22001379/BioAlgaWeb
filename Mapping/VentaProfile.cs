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
            // =========================
            // 1) LO QUE YA TENÍAS (CREAR)
            // =========================
            CreateMap<Venta, VentaDto>()
                .ForMember(d => d.Lineas, o => o.MapFrom(s =>
                    s.Detalles.Select(l => new VentaLineaCreate
                    {
                        IdProducto        = l.IdProducto,
                        Cantidad          = l.Cantidad,
                        PrecioUnitario    = l.PrecioUnitario,
                        DescuentoUnitario = l.DescuentoUnitario,
                        IvaUnitario       = l.IvaUnitario
                    }).ToList()
                ));

            CreateMap<VentaCreateRequest, Venta>()
                .ForMember(d => d.IdVenta,          o => o.Ignore())
                .ForMember(d => d.IdUsuario,        o => o.Ignore())
                .ForMember(d => d.ClienteId,        o => o.MapFrom(s => s.ClienteId))
                .ForMember(d => d.MetodoPago,       o => o.MapFrom(s => s.MetodoPago))
                .ForMember(d => d.EfectivoRecibido, o => o.MapFrom(s => s.EfectivoRecibido))
                .ForMember(d => d.Detalles,          o => o.Ignore())
                .ForMember(d => d.Subtotal,         o => o.Ignore())
                .ForMember(d => d.Impuestos,        o => o.Ignore())
                .ForMember(d => d.Total,            o => o.Ignore())
                .ForMember(d => d.Cambio,           o => o.Ignore())
                .ForMember(d => d.Estatus,          o => o.Ignore())
                .ForMember(d => d.FechaVenta,       o => o.Ignore());

            CreateMap<VentaLineaCreate, DetalleVenta>()
                .ForMember(d => d.IdDetalle, o => o.Ignore())
                .ForMember(d => d.IdVenta,   o => o.Ignore());

            // =====================================
            // 2) NUEVO: VER DETALLES / HISTORIAL
            // =====================================

            // ---- DetalleVenta -> VentaLineaDto ----
            CreateMap<DetalleVenta, VentaLineaDto>()
                .ForMember(d => d.IdDetalle,      o => o.MapFrom(s => s.IdDetalle))
                .ForMember(d => d.IdProducto,     o => o.MapFrom(s => s.IdProducto))
                .ForMember(d => d.ProductoNombre, o => o.MapFrom(s => s.Producto != null ? s.Producto.Nombre : string.Empty))
                .ForMember(d => d.CodigoSku,      o => o.MapFrom(s => s.Producto != null ? s.Producto.CodigoSku : null))
                .ForMember(d => d.Cantidad,       o => o.MapFrom(s => s.Cantidad))
                .ForMember(d => d.PrecioUnitario, o => o.MapFrom(s => s.PrecioUnitario))
                .ForMember(d => d.DescuentoUnitario, o => o.MapFrom(s => s.DescuentoUnitario))
                .ForMember(d => d.IvaUnitario,    o => o.MapFrom(s => s.IvaUnitario));

            // ---- Venta -> VentaResumenDto (para el historial) ----
            CreateMap<Venta, VentaResumenDto>()
                .ForMember(d => d.IdVenta,       o => o.MapFrom(s => s.IdVenta))
                .ForMember(d => d.FechaVenta,    o => o.MapFrom(s => s.FechaVenta))
                .ForMember(d => d.ClienteId,     o => o.MapFrom(s => s.ClienteId))
                .ForMember(d => d.ClienteNombre, o => o.MapFrom(s => NombreCliente(s.Cliente)))
                .ForMember(d => d.Subtotal,      o => o.MapFrom(s => s.Subtotal))
                .ForMember(d => d.Impuestos,     o => o.MapFrom(s => s.Impuestos))
                .ForMember(d => d.Total,         o => o.MapFrom(s => s.Total))
                .ForMember(d => d.MetodoPago,    o => o.MapFrom(s => s.MetodoPago))
                .ForMember(d => d.Estatus,       o => o.MapFrom(s => s.Estatus))
                .ForMember(d => d.Partidas,      o => o.MapFrom(s => s.Detalles != null ? s.Detalles.Count : 0))
                .ForMember(d => d.Unidades,      o => o.MapFrom(s => s.Detalles != null ? s.Detalles.Sum(x => x.Cantidad) : 0))
                .ForMember(d => d.IdUsuario,     o => o.MapFrom(s => s.IdUsuario))
                .ForMember(d => d.UsuarioNombre, o => o.MapFrom(s => s.Usuario != null ? s.Usuario.Nombre_Usuario : null));

            // ---- Venta -> VentaDetalleDto (encabezado + líneas) ----
            CreateMap<Venta, VentaDetalleDto>()
                .ForMember(d => d.IdVenta,         o => o.MapFrom(s => s.IdVenta))
                .ForMember(d => d.FechaVenta,      o => o.MapFrom(s => s.FechaVenta))
                .ForMember(d => d.ClienteId,       o => o.MapFrom(s => s.ClienteId))
                .ForMember(d => d.ClienteNombre,   o => o.MapFrom(s => NombreCliente(s.Cliente)))
                .ForMember(d => d.Subtotal,        o => o.MapFrom(s => s.Subtotal))
                .ForMember(d => d.Impuestos,       o => o.MapFrom(s => s.Impuestos))
                .ForMember(d => d.Total,           o => o.MapFrom(s => s.Total))
                .ForMember(d => d.EfectivoRecibido,o => o.MapFrom(s => s.EfectivoRecibido))
                .ForMember(d => d.Cambio,          o => o.MapFrom(s => s.Cambio))
                .ForMember(d => d.MetodoPago,      o => o.MapFrom(s => s.MetodoPago))
                .ForMember(d => d.Estatus,         o => o.MapFrom(s => s.Estatus))
                .ForMember(d => d.IdUsuario,       o => o.MapFrom(s => s.IdUsuario))
                .ForMember(d => d.UsuarioNombre,   o => o.MapFrom(s => s.Usuario != null ? s.Usuario.Nombre_Usuario : null))
                .ForMember(d => d.Partidas,        o => o.MapFrom(s => s.Detalles != null ? s.Detalles.Count : 0))
                .ForMember(d => d.Unidades,        o => o.MapFrom(s => s.Detalles != null ? s.Detalles.Sum(x => x.Cantidad) : 0))
                .ForMember(d => d.Detalles,        o => o.MapFrom(s => s.Detalles)); // usa el mapping de arriba
        }

        private static string? NombreCliente(Cliente? c)
        {
            if (c == null) return "Público en general";
            var parts = new[] { c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno }
                        .Where(p => !string.IsNullOrWhiteSpace(p));
            var full = string.Join(" ", parts).Trim();
            return string.IsNullOrWhiteSpace(full) ? "Público en general" : full;
        }
    }
}
