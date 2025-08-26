using BioAlga.Backend.Models.Enums;

public class CajaAperturaCreate
{
    public decimal FondoInicial { get; set; }
}

public class CajaMovimientoCreate
{
    public int IdCajaApertura { get; set; }
    public TipoCajaMovimiento Tipo { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal MontoEfectivo { get; set; }
    public int? IdVenta { get; set; }
}

public class CajaCorteCreate
{
    public int IdCajaApertura { get; set; }
    public decimal TotalEfectivoContado { get; set; }
}
