using System;
using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class DevolucionProfile : Profile
    {
        public DevolucionProfile()
        {
            // =============== ENTIDAD → DTO (lectura) ===============
            CreateMap<Devolucion, DevolucionDto>()
                .ForMember(d => d.Detalles, cfg => cfg.MapFrom(s => s.Detalles));

            CreateMap<DetalleDevolucion, DevolucionDetalleDto>();

            // =============== CREATE → ENTIDAD (escritura) ===============
            // Cabecera desde la petición de creación
            CreateMap<DevolucionCreateRequest, Devolucion>()
                // Fecha la deja EF o el servicio; por seguridad la inicializamos aquí.
                .ForMember(d => d.FechaDevolucion, cfg => cfg.MapFrom(_ => DateTime.Now))
                // Total se calcula en el servicio sumando las líneas
                .ForMember(d => d.TotalDevuelto, cfg => cfg.Ignore())
                // IdUsuario lo setea el servicio con el usuario logueado
                .ForMember(d => d.IdUsuario, cfg => cfg.Ignore())
                // Venta (FK) ya viene en el request (opcional)
                .ForMember(d => d.VentaId, cfg => cfg.MapFrom(s => s.VentaId))
                // Navegaciones no se asignan desde el request
                .ForMember(d => d.Venta, cfg => cfg.Ignore())
                .ForMember(d => d.Usuario, cfg => cfg.Ignore())
                .ForMember(d => d.Detalles, cfg => cfg.Ignore()); // se cargan con las líneas mapeadas

            // Renglón de detalle desde línea de creación
            CreateMap<DevolucionLineaCreate, DetalleDevolucion>()
                .ForMember(d => d.IdDetalle, cfg => cfg.Ignore())           // identity
                .ForMember(d => d.IdDevolucion, cfg => cfg.Ignore())        // lo setea el servicio
                .ForMember(d => d.Producto, cfg => cfg.Ignore())
                .ForMember(d => d.Devolucion, cfg => cfg.Ignore())
                // ImporteLineaTotal lo puede calcular el servicio (precio * cantidad)
                .ForMember(d => d.ImporteLineaTotal, cfg => cfg.Ignore());
        }
    }
}
