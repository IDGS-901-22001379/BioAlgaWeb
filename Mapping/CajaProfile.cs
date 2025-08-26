using AutoMapper;
using BioAlga.Backend.Models;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Mapping;

public class CajaProfile : Profile
{
    public CajaProfile()
    {
        // ===== Apertura =====
        CreateMap<CajaApertura, CajaAperturaDto>();
        CreateMap<CajaAperturaCreate, CajaApertura>();

        // ===== Movimiento =====
        CreateMap<CajaMovimiento, CajaMovimientoDto>();
        CreateMap<CajaMovimientoCreate, CajaMovimiento>();

        // ===== Corte =====
        CreateMap<CajaCorte, CajaCorteDto>();
        CreateMap<CajaCorteCreate, CajaCorte>();
    }
}
