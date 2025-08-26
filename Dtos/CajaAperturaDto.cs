using System;

namespace BioAlga.Backend.Dtos;

public class CajaAperturaDto
{
    public int IdCajaApertura { get; set; }
    public DateTime FechaApertura { get; set; }
    public int IdUsuario { get; set; }
    public decimal FondoInicial { get; set; }
    public bool Activa { get; set; }
}
