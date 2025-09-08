using AutoMapper;
using BioAlga.Backend.Models.Dashboard;
using BioAlga.Backend.Dtos.Dashboard;

namespace BioAlga.Backend.Mapping
{
    public class DashboardProfile : Profile
    {
        public DashboardProfile()
        {
            // Ventas resumen
            CreateMap<VentasResumen, VentasResumenDto>();

            // Top productos (unidades e ingreso)
            CreateMap<TopProducto, TopProductoDto>();

            // Top clientes
            CreateMap<TopCliente, TopClienteDto>();

            // Ventas por usuario
            CreateMap<VentasPorUsuario, VentasPorUsuarioDto>();

            // Devoluciones por usuario
            CreateMap<DevolucionesPorUsuario, DevolucionesPorUsuarioDto>();

            // Compras por proveedor
            CreateMap<ComprasPorProveedor, ComprasPorProveedorDto>();
        }
    }
}
