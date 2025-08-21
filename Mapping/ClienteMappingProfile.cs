using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class ClienteMappingProfile : Profile
    {
        public ClienteMappingProfile()
        {
            // ===== Modelo -> DTO =====
            CreateMap<Cliente, ClienteDto>()
                .ForMember(d => d.Id_Cliente,        o => o.MapFrom(s => s.IdCliente))
                .ForMember(d => d.Apellido_Paterno,  o => o.MapFrom(s => s.ApellidoPaterno))
                .ForMember(d => d.Apellido_Materno,  o => o.MapFrom(s => s.ApellidoMaterno))
                .ForMember(d => d.Tipo_Cliente,      o => o.MapFrom(s => s.TipoCliente))
                .ForMember(d => d.Fecha_Registro,    o => o.MapFrom(s => s.FechaRegistro));

            // ===== Crear DTO -> Modelo =====
            CreateMap<CrearClienteDto, Cliente>()
                .ForMember(d => d.IdCliente,         o => o.Ignore())
                .ForMember(d => d.ApellidoPaterno,   o => o.MapFrom(s => s.Apellido_Paterno))
                .ForMember(d => d.ApellidoMaterno,   o => o.MapFrom(s => s.Apellido_Materno))
                .ForMember(d => d.TipoCliente,       o => o.MapFrom(s => s.Tipo_Cliente))
                .ForMember(d => d.FechaRegistro,     o => o.MapFrom(_ => DateTime.UtcNow));

            // ===== Actualizar DTO -> Modelo =====
            CreateMap<ActualizarClienteDto, Cliente>()
                .ForMember(d => d.IdCliente,         o => o.Ignore())
                .ForMember(d => d.ApellidoPaterno,   o => o.MapFrom(s => s.Apellido_Paterno))
                .ForMember(d => d.ApellidoMaterno,   o => o.MapFrom(s => s.Apellido_Materno))
                .ForMember(d => d.TipoCliente,       o => o.MapFrom(s => s.Tipo_Cliente))
                .ForMember(d => d.FechaRegistro,     o => o.Ignore());

            
        }
    }
}
