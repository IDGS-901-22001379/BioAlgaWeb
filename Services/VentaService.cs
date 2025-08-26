using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Models.Enums;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VentaService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // =========================================================
        // Helper: calcula stock (Entradas - Salidas) de un producto
        // SIN usar Contains(ids) ni colecciones primitivas en la query.
        // =========================================================
        private async Task<int> GetStockAsync(int idProd, CancellationToken ct = default)
        {
            // Usamos int? + ?? 0 para evitar excepción si no hay filas.
            return await _db.InventarioMovimientos
                .AsNoTracking()
                .Where(m => m.IdProducto == idProd)
                .Select(m => (int?) (m.TipoMovimiento == "Salida" ? -m.Cantidad : m.Cantidad))
                .SumAsync(ct) ?? 0;
        }

        public async Task<VentaDto> RegistrarVentaAsync(int idUsuario, VentaCreateRequest req)
        {
            if (req.Lineas == null || req.Lineas.Count == 0)
                throw new ArgumentException("La venta requiere al menos una línea.");

            var ids = req.Lineas.Select(l => l.IdProducto).Distinct().ToList();

            // =========================================================
            // 1) Validar existencia/estatus de productos (sin Contains)
            //    Se consulta por id, uno por uno.
            // =========================================================
            foreach (var idProd in ids)
            {
                var prod = await _db.Productos
                    .AsNoTracking()
                    .Where(p => p.IdProducto == idProd)
                    .Select(p => new { p.IdProducto, p.Estatus })
                    .FirstOrDefaultAsync();

                if (prod is null)
                    throw new InvalidOperationException($"El producto {idProd} no existe.");

                if (!string.Equals(prod.Estatus, "Activo", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"El producto {idProd} está inactivo.");
            }

            // =========================================================
            // 2) Verificar stock disponible (sin Contains)
            //    Una query por producto usando el helper.
            // =========================================================
            var stockPorProd = new Dictionary<int, int>();
            foreach (var idProd in ids)
                stockPorProd[idProd] = await GetStockAsync(idProd);

            foreach (var l in req.Lineas)
            {
                var stock = stockPorProd.TryGetValue(l.IdProducto, out var s) ? s : 0;
                if (stock < l.Cantidad)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para el producto {l.IdProducto}. Actual: {stock}");
            }

            // =========================================================
            // 3) Totales
            // =========================================================
            decimal subtotal  = req.Lineas.Sum(l => (l.PrecioUnitario - l.DescuentoUnitario) * l.Cantidad);
            decimal impuestos = req.Lineas.Sum(l => l.IvaUnitario * l.Cantidad);
            decimal total     = subtotal + impuestos;

            decimal? cambio = null;
            if (req.MetodoPago is MetodoPago.Efectivo or MetodoPago.Mixto)
            {
                var recibido = req.EfectivoRecibido ?? 0m;
                if (req.MetodoPago == MetodoPago.Efectivo && recibido < total)
                    throw new InvalidOperationException("Efectivo insuficiente.");
                cambio = Math.Max(0m, recibido - total);
            }

            // =========================================================
            // 4) Persistencia (Transacción)
            // =========================================================
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var venta = new Venta
                {
                    ClienteId        = req.ClienteId,
                    FechaVenta       = DateTime.UtcNow,
                    Subtotal         = subtotal,
                    Impuestos        = impuestos,
                    Total            = total,
                    MetodoPago       = req.MetodoPago,
                    EfectivoRecibido = req.EfectivoRecibido,
                    Cambio           = cambio,
                    IdUsuario        = idUsuario,
                    Estatus          = EstatusVenta.Pagada
                };

                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync();

                var detalles = req.Lineas.Select(l => new DetalleVenta
                {
                    IdVenta           = venta.IdVenta,
                    IdProducto        = l.IdProducto,
                    Cantidad          = l.Cantidad,
                    PrecioUnitario    = l.PrecioUnitario,
                    DescuentoUnitario = l.DescuentoUnitario,
                    IvaUnitario       = l.IvaUnitario
                }).ToList();

                _db.DetalleVentas.AddRange(detalles);
                await _db.SaveChangesAsync();

                // Movimientos de inventario (Salida por venta)
                var movsSalida = detalles.Select(d => new InventarioMovimiento
                {
                    IdProducto     = d.IdProducto,
                    TipoMovimiento = "Salida",
                    Cantidad       = d.Cantidad,
                    Fecha          = DateTime.UtcNow,
                    OrigenTipo     = "Venta",
                    OrigenId       = venta.IdVenta,
                    IdUsuario      = idUsuario,
                    Referencia     = $"Venta #{venta.IdVenta}"
                });
                _db.InventarioMovimientos.AddRange(movsSalida);
                await _db.SaveChangesAsync();

                // Caja: ingreso cuando es Efectivo
                if (req.MetodoPago == MetodoPago.Efectivo)
                {
                    var apertura = await _db.CajaAperturas
                        .Where(a => a.IdUsuario == idUsuario && a.Activa)
                        .OrderByDescending(a => a.FechaApertura)
                        .FirstOrDefaultAsync();

                    if (apertura != null)
                    {
                        _db.CajaMovimientos.Add(new CajaMovimiento
                        {
                            IdCajaApertura = apertura.IdCajaApertura,
                            Fecha          = DateTime.UtcNow,
                            Tipo           = TipoCajaMovimiento.Ingreso,
                            Concepto       = $"Venta #{venta.IdVenta}",
                            MontoEfectivo  = total,
                            IdVenta        = venta.IdVenta,
                            IdUsuario      = idUsuario
                        });
                        await _db.SaveChangesAsync();
                    }
                }

                await tx.CommitAsync();

                venta.Detalle = detalles;
                return _mapper.Map<VentaDto>(venta);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelarVentaAsync(int idUsuario, int idVenta)
        {
            var venta = await _db.Ventas
                .Include(v => v.Detalle)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta == null) return false;
            if (venta.Estatus == EstatusVenta.Cancelada) return true;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Revertir inventario (Entrada)
                var entradas = venta.Detalle.Select(d => new InventarioMovimiento
                {
                    IdProducto     = d.IdProducto,
                    TipoMovimiento = "Entrada",
                    Cantidad       = d.Cantidad,
                    Fecha          = DateTime.UtcNow,
                    OrigenTipo     = "Venta",
                    OrigenId       = venta.IdVenta,
                    IdUsuario      = idUsuario,
                    Referencia     = $"Cancelación venta #{venta.IdVenta}"
                });
                _db.InventarioMovimientos.AddRange(entradas);

                // Caja: egreso si fue efectivo
                if (venta.MetodoPago == MetodoPago.Efectivo)
                {
                    var apertura = await _db.CajaAperturas
                        .Where(a => a.IdUsuario == idUsuario && a.Activa)
                        .FirstOrDefaultAsync();

                    if (apertura != null)
                    {
                        _db.CajaMovimientos.Add(new CajaMovimiento
                        {
                            IdCajaApertura = apertura.IdCajaApertura,
                            Fecha          = DateTime.UtcNow,
                            Tipo           = TipoCajaMovimiento.Egreso,
                            Concepto       = $"Cancelación venta #{venta.IdVenta}",
                            MontoEfectivo  = venta.Total,
                            IdVenta        = venta.IdVenta,
                            IdUsuario      = idUsuario
                        });
                    }
                }

                venta.Estatus = EstatusVenta.Cancelada;
                _db.Ventas.Update(venta);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
