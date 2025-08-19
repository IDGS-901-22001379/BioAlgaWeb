using AutoMapper;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Mapping
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            // ==========================
            // Entidad → DTO
            // ==========================
            CreateMap<Cliente, ClienteDto>();

            // ==========================
            // CrearClienteDto → Cliente
            // ==========================
            CreateMap<CrearClienteDto, Cliente>()
                .ForMember(d => d.Fecha_Registro, opt => opt.Ignore()) // lo llena MySQL
                .ForMember(d => d.Estado, opt => opt.MapFrom(src => src.Estado ?? "Activo"));

            // ==========================
            // ActualizarClienteDto → Cliente
            // Solo mapea propiedades no nulas
            // ==========================
            CreateMap<ActualizarClienteDto, Cliente>()
                .ForAllMembers(opt => opt.Condition(
                    (src, dest, srcMember) => srcMember != null
                ));
        }
    }
}
