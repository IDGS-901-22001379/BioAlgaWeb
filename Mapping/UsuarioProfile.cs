// Mapping/UsuarioProfile.cs
using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            // Usuario -> UsuarioDto
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(d => d.Id_Usuario,       opt => opt.MapFrom(s => s.Id_Usuario))
                .ForMember(d => d.Nombre_Usuario,   opt => opt.MapFrom(s => s.Nombre_Usuario))
                .ForMember(d => d.Rol,              opt => opt.MapFrom(s => s.Rol != null ? s.Rol.Nombre : string.Empty))
                .ForMember(d => d.Id_Rol,           opt => opt.MapFrom(s => s.Id_Rol))
                .ForMember(d => d.Id_Empleado,      opt => opt.MapFrom(s => s.Id_Empleado))
                .ForMember(d => d.Nombre_Empleado,  opt => opt.MapFrom(s => s.Empleado != null ? s.Empleado.Nombre : null))
                .ForMember(d => d.Apellido_Paterno, opt => opt.MapFrom(s => s.Empleado != null ? s.Empleado.Apellido_Paterno : null))
                .ForMember(d => d.Apellido_Materno, opt => opt.MapFrom(s => s.Empleado != null ? s.Empleado.Apellido_Materno : null))
                .ForMember(d => d.Activo,           opt => opt.MapFrom(s => s.Activo))
                .ForMember(d => d.Ultimo_Login,     opt => opt.MapFrom(s => s.Ultimo_Login))
                .ForMember(d => d.Fecha_Registro,   opt => opt.MapFrom(s => s.Fecha_Registro));

            // UsuarioCreateRequest -> Usuario (NO mapear Contrasena aquí; se hashea en el servicio/controlador)
            CreateMap<UsuarioCreateRequest, Usuario>()
                .ForMember(d => d.Contrasena,   opt => opt.Ignore())
                .ForMember(d => d.Fecha_Registro, opt => opt.Ignore()) // se setea a mano
                .ForMember(d => d.Ultimo_Login, opt => opt.Ignore());

            // UsuarioUpdateRequest -> Usuario (ignorar nulos para updates parciales; Contrasena se maneja aparte)
            CreateMap<UsuarioUpdateRequest, Usuario>()
                .ForMember(d => d.Contrasena, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
