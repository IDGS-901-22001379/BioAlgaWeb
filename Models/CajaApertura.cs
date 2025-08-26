using System;

namespace BioAlga.Backend.Models;

public class CajaApertura
{
    public int IdCajaApertura { get; set; }
    public DateTime FechaApertura { get; set; } = DateTime.UtcNow;

    public int IdUsuario { get; set; }               // quien abre
    public decimal FondoInicial { get; set; }        // efectivo inicial

    public bool Activa { get; set; } = true;         // se cierra con el corte

    // Navs
    public Usuario Usuario { get; set; } = default!;
}
