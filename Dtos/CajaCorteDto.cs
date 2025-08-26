using System;

namespace BioAlga.Backend.Dtos;

public class CajaCorteDto
{
    public int IdCajaCorte { get; set; }
    public int IdCajaApertura { get; set; }
    public DateTime FechaCorte { get; set; }

    public decimal TotalEfectivoEsperado { get; set; }
    public decimal TotalEfectivoContado { get; set; }
    public decimal Diferencia { get; set; }

    public int IdUsuario { get; set; }
}
