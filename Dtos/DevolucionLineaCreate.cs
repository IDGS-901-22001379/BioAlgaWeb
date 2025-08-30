using System.ComponentModel.DataAnnotations;

namespace BioAlga.Backend.Dtos
{
    public class DevolucionLineaCreate
    {
        [Required]
        public int IdProducto { get; set; }

        /// <summary>
        /// Cantidad devuelta (entero positivo)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        /// <summary>
        /// Importe total de la línea (capturado a mano) = precio pagado por esa cantidad.
        /// </summary>
        [Range(typeof(decimal), "0.00", "9999999999.99")]
        public decimal ImporteLineaTotal { get; set; }
    }
}
