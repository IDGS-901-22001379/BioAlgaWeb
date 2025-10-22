using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BioAlga.Backend.Models.Enums;

namespace BioAlga.Backend.Services
{
    public class CorteService : ICorteService
    {
        private readonly ApplicationDbContext _db;

        public CorteService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CorteResumenDto?> ObtenerResumenTurnoAsync(int idTurno)
        {
            // 1) Turno
            var turno = await _db.CajaTurnos.AsNoTracking().FirstOrDefaultAsync(t => t.IdTurno == idTurno);
            if (turno is null) return null;

            var desde = turno.Apertura;
            var hasta = turno.Cierre ?? DateTime.UtcNow;

            // 2) Ventas del usuario dentro de la ventana
            var ventasQuery = _db.Ventas.AsNoTracking()
                .Where(v => v.IdUsuario == turno.IdUsuario
                        && v.Estatus == EstatusVenta.Pagada
                        && v.FechaVenta >= desde && v.FechaVenta < hasta);

            var ventasTotales = await ventasQuery.SumAsync(v => (decimal?)v.Total) ?? 0m;
            var numTickets = await ventasQuery.CountAsync();

            // 3) Efectivo neto de tickets (efectivo_recibido - cambio)
            var ventasEfectivoNeto = await ventasQuery
                .SumAsync(v => (decimal?)(v.EfectivoRecibido - v.Cambio)) ?? 0m;

            // 4) Desglose por método
            //    Si hay registros en venta_pagos para esas ventas, usar ese desglose.
            //    Si no, agrupar por ventas.metodo_pago.
            var ventaIds = await ventasQuery.Select(v => v.IdVenta).ToListAsync();

            List<CorteMetodoDto> porMetodo;
            if (ventaIds.Count > 0 &&
                await _db.VentaPagos.AsNoTracking().AnyAsync(p => ventaIds.Contains(p.IdVenta)))
            {
                porMetodo = await _db.VentaPagos.AsNoTracking()
                    .Where(p => ventaIds.Contains(p.IdVenta))
                    .GroupBy(p => p.Metodo)
                    .Select(g => new CorteMetodoDto
                    {
                        Metodo = g.Key,
                        Monto = g.Sum(x => x.Monto)
                    })
                    .ToListAsync();
            }
            else
            {
                porMetodo = await ventasQuery
                    .GroupBy(v => v.MetodoPago)
                    .Select(g => new CorteMetodoDto
                    {
                        Metodo = g.Key.ToString(),
                        Monto = g.Sum(x => x.Total)
                    })
                    .ToListAsync();
            }

            // 5) Devoluciones del usuario en la ventana
            var devolucionesTotal = await _db.Devoluciones.AsNoTracking()
                .Where(d => d.IdUsuario == turno.IdUsuario &&
                            d.FechaDevolucion >= desde && d.FechaDevolucion < hasta)
                .SumAsync(d => (decimal?)d.TotalDevuelto) ?? 0m;

            // 6) Movimientos manuales del turno
            var movs = await _db.CajaMovimientos.AsNoTracking()
                .Where(m => m.IdTurno == turno.IdTurno)
                .ToListAsync();

            var entradas = movs.Where(m => m.Tipo == "Ingreso").Sum(m => m.Monto);
            var salidas = movs.Where(m => m.Tipo == "Egreso").Sum(m => m.Monto);

            // 7) Efectivo esperado = SaldoInicial + VentasEfectivoNeto + Entradas - Salidas - Devoluciones
            var esperado = turno.SaldoInicial + ventasEfectivoNeto + entradas - salidas - devolucionesTotal;

            // 8) Armar DTO
            return new CorteResumenDto
            {
                Id_Turno = turno.IdTurno,
                Id_Caja = turno.IdCaja,
                Id_Usuario = turno.IdUsuario,
                Apertura = turno.Apertura,
                Cierre_Usado = hasta,
                Num_Tickets = numTickets,
                Ventas_Totales = ventasTotales,
                Ventas_Por_Metodo = porMetodo.OrderBy(x => x.Metodo).ToList(),
                Ventas_Efectivo_Neto = ventasEfectivoNeto,
                Entradas_Efectivo = entradas,
                Salidas_Efectivo = salidas,
                Devoluciones_Total = devolucionesTotal,
                Saldo_Inicial = turno.SaldoInicial,
                Efectivo_Esperado = esperado
            };
        }
    }
}
