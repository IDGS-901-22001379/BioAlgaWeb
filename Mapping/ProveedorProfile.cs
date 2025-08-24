using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class ProveedorProfile : Profile
    {
        public ProveedorProfile()
        {
            CreateMap<Proveedor, ProveedorDto>()
                .ForMember(d => d.Id_Proveedor, o => o.MapFrom(s => s.IdProveedor))
                .ForMember(d => d.Nombre_Empresa, o => o.MapFrom(s => s.NombreEmpresa))
                .ForMember(d => d.Codigo_Postal, o => o.MapFrom(s => s.CodigoPostal))
                .ForMember(d => d.Created_At, o => o.MapFrom(s => s.CreatedAt));

            CreateMap<CrearProveedorDto, Proveedor>()
                .ForMember(d => d.NombreEmpresa, o => o.MapFrom(s => s.Nombre_Empresa))
                .ForMember(d => d.CodigoPostal, o => o.MapFrom(s => s.Codigo_Postal))
                .ForMember(d => d.Estatus, o => o.MapFrom(_ => "Activo"));

            CreateMap<ActualizarProveedorDto, Proveedor>()
                .ForMember(d => d.NombreEmpresa, o => o.MapFrom(s => s.Nombre_Empresa))
                .ForMember(d => d.CodigoPostal, o => o.MapFrom(s => s.Codigo_Postal));
        }
    }
}
