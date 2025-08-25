using System;

namespace BioAlga.Backend.Dtos
{
    public class AjusteInventarioDto
    {
        public int Id_Producto { get; set; }
        public int Cantidad { get; set; }           // > 0
        public int Id_Usuario { get; set; }         // quién realizó el ajuste
        public string? Motivo { get; set; }         // referencia/motivo opcional
    }

    public class MovimientoResultDto
    {
        public int Id_Movimiento { get; set; }
        public int Id_Producto { get; set; }
        public string Tipo_Movimiento { get; set; } = ""; // Entrada/Salida
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Origen_Tipo { get; set; } = "Ajuste";
        public int? Origen_Id { get; set; } = null;
        public string? Referencia { get; set; }
        public int Stock_Despues { get; set; }      // stock resultante
    }

    public class StockActualResponse
    {
        public int Id_Producto { get; set; }
        public int Stock { get; set; }
    }

    public class KardexItemDto
    {
        public DateTime Fecha { get; set; }
        public string  Tipo { get; set; } = ""; // "Entrada" | "Salida" | "Ajuste"
        public int     Cantidad { get; set; }   // para Salida ya va en negativo
        public int?    Saldo { get; set; }      // saldo acumulado
        public string  Origen { get; set; } = "";
    }
}
