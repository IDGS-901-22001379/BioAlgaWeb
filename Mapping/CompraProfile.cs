using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping;

public class CompraProfile : Profile
{
    public CompraProfile()
    {
        CreateMap<Compra, CompraDto>()
            .ForMember(d => d.Id_Compra, m => m.MapFrom(s => s.IdCompra))
            .ForMember(d => d.Proveedor_Id, m => m.MapFrom(s => s.ProveedorId))
            .ForMember(d => d.Proveedor_Texto, m => m.MapFrom(s => s.ProveedorTexto))
            .ForMember(d => d.Fecha_Compra, m => m.MapFrom(s => s.FechaCompra))
            .ForMember(d => d.Estado, m => m.MapFrom(s => s.Detalles.Any() ? "Borrador" : "Borrador"));

        CreateMap<DetalleCompra, DetalleCompraDto>()
            .ForMember(d => d.Id_Detalle, m => m.MapFrom(s => s.IdDetalle))
            .ForMember(d => d.Id_Producto, m => m.MapFrom(s => s.IdProducto))
            .ForMember(d => d.Producto, m => m.MapFrom(s => s.Producto != null ? s.Producto.Nombre : string.Empty))
            .ForMember(d => d.SKU, m => m.MapFrom(s => s.Producto != null ? s.Producto.CodigoSku : string.Empty))
            .ForMember(d => d.Costo_Unitario, m => m.MapFrom(s => s.CostoUnitario))
            .ForMember(d => d.Iva_Unitario, m => m.MapFrom(s => s.IvaUnitario));
    }
}
