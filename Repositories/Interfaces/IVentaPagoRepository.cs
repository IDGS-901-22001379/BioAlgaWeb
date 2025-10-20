using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IVentaPagoRepository
    {
        Task<IReadOnlyList<VentaPago>> GetByVentaAsync(int idVenta);
        Task<VentaPago> AddAsync(VentaPago pago);
        Task<bool> DeleteAsync(int idPago);

        // Auxiliar: suma por método
        Task<decimal> TotalPorMetodoAsync(int idVenta, string metodo);
    }
}
