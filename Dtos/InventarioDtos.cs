namespace BioAlga.Backend.Dtos
{
    public class StockActualResponse
    {
        public int Id_Producto { get; set; }
        public int Stock { get; set; }
    }

    public class KardexItemDto
    {
        public DateTime Fecha { get; set; }
        public string  Tipo { get; set; } = ""; // "Entrada" | "Salida" | "Ajuste"
        public int     Cantidad { get; set; }   // Salida va en negativo
        public int?    Saldo { get; set; }      // saldo acumulado
        public string  Origen { get; set; } = "";
    }
}
