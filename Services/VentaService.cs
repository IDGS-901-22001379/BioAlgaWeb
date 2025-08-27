using System.ComponentModel.DataAnnotations;
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

        public VentaService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<VentaDto> RegistrarVentaAsync(int idUsuario, VentaCreateRequest req)
        {
            // ===== Validaciones de entrada =====
            if (req.Lineas is null || req.Lineas.Count == 0)
                throw new ValidationException("La venta debe incluir al menos un producto.");

            if ((req.MetodoPago == MetodoPago.Efectivo || req.MetodoPago == MetodoPago.Mixto) &&
                req.EfectivoRecibido is null)
                throw new ValidationException("Efectivo recibido es requerido para método Efectivo/Mixto.");

            // ===== Preparar venta (cabecera) =====
            var venta = new Venta
            {
                IdUsuario        = idUsuario,
                ClienteId        = req.ClienteId,
                FechaVenta       = DateTime.UtcNow,
                MetodoPago       = req.MetodoPago,
                EfectivoRecibido = req.EfectivoRecibido,
                Estatus          = EstatusVenta.Pagada, // ajusta a tu regla si manejas "Pendiente"
            };

            // ===== Resolver líneas + validar productos y stock =====
            decimal subtotal = 0m, impuestos = 0m;

            foreach (var l in req.Lineas)
            {
                var prod = await _db.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.IdProducto == l.IdProducto)
                    ?? throw new ValidationException($"Producto {l.IdProducto} no existe.");

                if (l.Cantidad <= 0)
                    throw new ValidationException("La cantidad debe ser mayor a 0.");

                // --- Chequeo de stock (existencia aproximada) ---
                // SUMA(Entrada/Ajuste+) - SUMA(Salida)
                var existencia = await _db.InventarioMovimientos
                    .Where(i => i.IdProducto == l.IdProducto)
                    .SumAsync(i =>
                        i.TipoMovimiento == nameof(TipoMovimiento.Salida) ? -i.Cantidad :
                        i.TipoMovimiento == nameof(TipoMovimiento.Entrada) ?  i.Cantidad :
                        // Ajuste: positivo o negativo según convención. Aquí se asume positivo.
                        i.Cantidad
                    );

                if (existencia < l.Cantidad)
                    throw new ValidationException($"Stock insuficiente para el producto {l.IdProducto}. Existencia: {existencia}");

                // Si el precio viene del front (según tipo de cliente), lo tomamos.
                // Si prefieres resolverlo aquí con tu tabla producto_precios vigente, hazlo antes.
                var precioUnit = l.PrecioUnitario;

                var linea = new DetalleVenta
                {
                    IdProducto        = l.IdProducto,
                    Cantidad          = l.Cantidad,
                    PrecioUnitario    = precioUnit,
                    DescuentoUnitario = l.DescuentoUnitario,
                    IvaUnitario       = l.IvaUnitario
                };

                venta.Detalle.Add(linea);

                // Totales
                var baseLinea = (precioUnit - l.DescuentoUnitario) * l.Cantidad;
                subtotal  += baseLinea;
                impuestos += (l.IvaUnitario * l.Cantidad);
            }

            venta.Subtotal = decimal.Round(subtotal, 2);
            venta.Impuestos = decimal.Round(impuestos, 2);
            venta.Total = decimal.Round(venta.Subtotal + venta.Impuestos, 2);

            if (venta.MetodoPago == MetodoPago.Efectivo || venta.MetodoPago == MetodoPago.Mixto)
            {
                var efectivo = venta.EfectivoRecibido ?? 0m;
                if (efectivo < venta.Total)
                    throw new ValidationException("El efectivo recibido no cubre el total.");
                venta.Cambio = decimal.Round(efectivo - venta.Total, 2);
            }

            // ===== Persistir con transacción + movimientos de inventario =====
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync(); // genera id_venta

                // Movimientos: SALIDA por cada línea
                foreach (var d in venta.Detalle)
                {
                    _db.InventarioMovimientos.Add(new InventarioMovimiento
                    {
                        IdProducto     = d.IdProducto,
                        Cantidad       = d.Cantidad,
                        Fecha          = venta.FechaVenta,
                        TipoMovimiento = nameof(TipoMovimiento.Salida),
                        OrigenTipo     = nameof(OrigenMovimiento.Venta),
                        OrigenId       = venta.IdVenta,
                        IdUsuario      = idUsuario,
                        Referencia     = $"Venta #{venta.IdVenta}"
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            // ===== Proyectar a DTO ===== (sustituible por AutoMapper)
            return new VentaDto
            {
                IdVenta          = venta.IdVenta,
                ClienteId        = venta.ClienteId,
                FechaVenta       = venta.FechaVenta,
                Subtotal         = venta.Subtotal,
                Impuestos        = venta.Impuestos,
                Total            = venta.Total,
                MetodoPago       = venta.MetodoPago,
                EfectivoRecibido = venta.EfectivoRecibido,
                Cambio           = venta.Cambio,
                Estatus          = venta.Estatus.ToString(),
                Lineas = venta.Detalle.Select(d => new VentaLineaCreate
                {
                    IdProducto        = d.IdProducto,
                    Cantidad          = d.Cantidad,
                    PrecioUnitario    = d.PrecioUnitario,
                    DescuentoUnitario = d.DescuentoUnitario,
                    IvaUnitario       = d.IvaUnitario
                }).ToList()
            };
        }

        public async Task<bool> CancelarVentaAsync(int idUsuario, int idVenta)
        {
            var venta = await _db.Ventas
                .Include(v => v.Detalle)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta is null) return false;
            if (venta.Estatus == EstatusVenta.Cancelada) return true;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Reponer inventario con ENTRADA
                foreach (var d in venta.Detalle)
                {
                    _db.InventarioMovimientos.Add(new InventarioMovimiento
                    {
                        IdProducto     = d.IdProducto,
                        Cantidad       = d.Cantidad,
                        Fecha          = DateTime.UtcNow,
                        TipoMovimiento = nameof(TipoMovimiento.Entrada),
                        OrigenTipo     = nameof(OrigenMovimiento.Venta),
                        OrigenId       = venta.IdVenta,
                        IdUsuario      = idUsuario,
                        Referencia     = $"Cancelación venta #{venta.IdVenta}"
                    });
                }

                venta.Estatus = EstatusVenta.Cancelada;
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return true;
        }
    }
}
