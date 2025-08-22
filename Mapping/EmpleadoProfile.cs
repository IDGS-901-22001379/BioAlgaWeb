using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping
{
    public class EmpleadoProfile : Profile
    {
        public EmpleadoProfile()
        {
            // ===== Modelo -> DTO =====
            CreateMap<Empleado, EmpleadoDto>()
                .ForMember(d => d.Id_Empleado, o => o.MapFrom(s => s.Id_Empleado))
                .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Nombre))
                .ForMember(d => d.Apellido_Paterno, o => o.MapFrom(s => s.Apellido_Paterno))
                .ForMember(d => d.Apellido_Materno, o => o.MapFrom(s => s.Apellido_Materno))
                .ForMember(d => d.Curp, o => o.MapFrom(s => s.Curp))
                .ForMember(d => d.Rfc, o => o.MapFrom(s => s.Rfc))
                .ForMember(d => d.Correo, o => o.MapFrom(s => s.Correo))
                .ForMember(d => d.Telefono, o => o.MapFrom(s => s.Telefono))
                .ForMember(d => d.Puesto, o => o.MapFrom(s => s.Puesto))
                .ForMember(d => d.Salario, o => o.MapFrom(s => s.Salario))
                .ForMember(d => d.Fecha_Ingreso, o => o.MapFrom(s => s.Fecha_Ingreso))
                .ForMember(d => d.Fecha_Baja, o => o.MapFrom(s => s.Fecha_Baja))
                .ForMember(d => d.Estatus, o => o.MapFrom(s => s.Estatus))
                .ForMember(d => d.Created_At, o => o.MapFrom(s => s.Created_At))
                .ForMember(d => d.Updated_At, o => o.MapFrom(s => s.Updated_At));

            // ===== Crear DTO -> Modelo =====
            CreateMap<CrearEmpleadoDto, Empleado>()
                .ForMember(d => d.Id_Empleado, o => o.Ignore()) // lo genera la BD
                .ForMember(d => d.Created_At, o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.Updated_At, o => o.MapFrom(_ => DateTime.UtcNow));

            // ===== Actualizar DTO -> Modelo =====
            CreateMap<ActualizarEmpleadoDto, Empleado>()
                .ForMember(d => d.Updated_At, o => o.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
