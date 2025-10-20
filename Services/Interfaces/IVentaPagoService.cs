using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IVentaPagoService
    {
        Task<IReadOnlyList<VentaPagoDto>> ListarPorVentaAsync(int idVenta);
        Task<VentaPagoDto> CrearAsync(CrearVentaPagoDto dto);
        Task<bool> EliminarAsync(int idPago);

        Task<decimal> TotalPorMetodoAsync(int idVenta, string metodo); // Efectivo/Tarjeta/Transferencia/Otro
    }
}
