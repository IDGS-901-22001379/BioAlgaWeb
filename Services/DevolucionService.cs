using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class DevolucionService : IDevolucionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public DevolucionService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> RegistrarDevolucionAsync(int idUsuario, DevolucionCreateRequest req)
        {
            var venta = await _db.Ventas.Include(v => v.Detalle)
                .FirstOrDefaultAsync(v => v.IdVenta == req.IdVenta);

            if (venta == null)
                throw new InvalidOperationException("La venta no existe.");

            if (venta.Estatus != Models.Enums.EstatusVenta.Pagada)
                throw new InvalidOperationException("Solo se pueden devolver ventas 'Pagada'.");

            decimal subtotal = req.Lineas.Sum(l => l.PrecioUnitario * l.Cantidad);
            decimal impuestos = req.Lineas.Sum(l => l.IvaUnitario * l.Cantidad);
            decimal total = subtotal + impuestos;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var devolucion = new Devolucion
                {
                    IdVenta = req.IdVenta,
                    Fecha = DateTime.UtcNow,
                    Motivo = req.Motivo,
                    ReingresaInventario = req.ReingresaInventario,
                    IdUsuario = idUsuario,
                    Subtotal = subtotal,
                    Impuestos = impuestos,
                    Total = total
                };
                _db.Devoluciones.Add(devolucion);
                await _db.SaveChangesAsync();

                var detalles = req.Lineas.Select(l => new DetalleDevolucion
                {
                    IdDevolucion = devolucion.IdDevolucion,
                    IdProducto = l.IdProducto,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = l.PrecioUnitario,
                    IvaUnitario = l.IvaUnitario
                }).ToList();
                _db.DetalleDevoluciones.AddRange(detalles);
                await _db.SaveChangesAsync();

                // Inventario
                string tipoMov = req.ReingresaInventario ? "Entrada" : "Ajuste";
                var movs = detalles.Select(d => new InventarioMovimiento
                {
                    IdProducto = d.IdProducto,
                    TipoMovimiento = tipoMov,
                    Cantidad = d.Cantidad,
                    Fecha = DateTime.UtcNow,
                    OrigenTipo = "Devolucion",
                    OrigenId = devolucion.IdDevolucion,
                    IdUsuario = idUsuario,
                    Referencia = $"Devolución #{devolucion.IdDevolucion}"
                });
                _db.InventarioMovimientos.AddRange(movs);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return devolucion.IdDevolucion;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
