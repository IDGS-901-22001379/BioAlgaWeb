using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Models.Enums;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _db;
        private readonly IVentaRepository _repo;
        private readonly IMapper _mapper;

        public VentaService(ApplicationDbContext db, IVentaRepository repo, IMapper mapper)
        {
            _db = db;
            _repo = repo;
            _mapper = mapper;
        }

        // ============================
        // HISTORIAL (paginado)
        // ============================
        public async Task<PagedResponse<VentaResumenDto>> BuscarVentasAsync(VentaQueryParams qp)
        {
            var (items, total) = await _repo.SearchAsync(qp);
            var list = _mapper.Map<List<VentaResumenDto>>(items);

            var page = qp.Page <= 0 ? 1 : qp.Page;
            var pageSize = qp.PageSize <= 0 ? 10 : qp.PageSize;

            return new PagedResponse<VentaResumenDto>
            {
                Items = list,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // ============================
        // DETALLE DE UNA VENTA
        // ============================
        public async Task<VentaDetalleDto?> ObtenerVentaPorIdAsync(int idVenta)
        {
            var venta = await _repo.GetByIdWithDetailsAsync(idVenta);
            if (venta is null) return null;

            return _mapper.Map<VentaDetalleDto>(venta);
        }

        // ============================
        // REGISTRAR VENTA
        // ============================
        public async Task<VentaDto> RegistrarVentaAsync(int idUsuario, VentaCreateRequest req)
        {
            if (req.Lineas is null || req.Lineas.Count == 0)
                throw new ValidationException("La venta debe incluir al menos un producto.");

            if ((req.MetodoPago == MetodoPago.Efectivo || req.MetodoPago == MetodoPago.Mixto) &&
                req.EfectivoRecibido is null)
                throw new ValidationException("Efectivo recibido es requerido para método Efectivo/Mixto.");

            var venta = new Venta
            {
                IdUsuario        = idUsuario,
                ClienteId        = req.ClienteId,
                FechaVenta       = DateTime.UtcNow,
                MetodoPago       = req.MetodoPago,
                EfectivoRecibido = req.EfectivoRecibido,
                Estatus          = EstatusVenta.Pagada
            };

            decimal subtotal = 0m, impuestos = 0m;

            foreach (var l in req.Lineas)
            {
                var prod = await _db.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.IdProducto == l.IdProducto)
                    ?? throw new ValidationException($"Producto {l.IdProducto} no existe.");

                if (l.Cantidad <= 0)
                    throw new ValidationException("La cantidad debe ser mayor a 0.");

                var existencia = await _db.InventarioMovimientos
                    .Where(i => i.IdProducto == l.IdProducto)
                    .SumAsync(i =>
                        i.TipoMovimiento == nameof(TipoMovimiento.Salida) ? -i.Cantidad :
                        i.TipoMovimiento == nameof(TipoMovimiento.Entrada) ?  i.Cantidad :
                        i.Cantidad
                    );

                if (existencia < l.Cantidad)
                    throw new ValidationException($"Stock insuficiente para el producto {l.IdProducto}. Existencia: {existencia}");

                var precioUnit = l.PrecioUnitario;

                var linea = new DetalleVenta
                {
                    IdProducto        = l.IdProducto,
                    Cantidad          = l.Cantidad,
                    PrecioUnitario    = precioUnit,
                    DescuentoUnitario = l.DescuentoUnitario,
                    IvaUnitario       = l.IvaUnitario
                };

                venta.Detalles.Add(linea);

                var baseLinea = (precioUnit - l.DescuentoUnitario) * l.Cantidad;
                subtotal  += baseLinea;
                impuestos += (l.IvaUnitario * l.Cantidad);
            }

            venta.Subtotal  = decimal.Round(subtotal, 2);
            venta.Impuestos = decimal.Round(impuestos, 2);
            venta.Total     = decimal.Round(venta.Subtotal + venta.Impuestos, 2);

            if (venta.MetodoPago == MetodoPago.Efectivo || venta.MetodoPago == MetodoPago.Mixto)
            {
                var efectivo = venta.EfectivoRecibido ?? 0m;
                if (efectivo < venta.Total)
                    throw new ValidationException("El efectivo recibido no cubre el total.");
                venta.Cambio = decimal.Round(efectivo - venta.Total, 2);
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync();

                foreach (var d in venta.Detalles)
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
                Lineas = venta.Detalles.Select(d => new VentaLineaCreate
                {
                    IdProducto        = d.IdProducto,
                    Cantidad          = d.Cantidad,
                    PrecioUnitario    = d.PrecioUnitario,
                    DescuentoUnitario = d.DescuentoUnitario,
                    IvaUnitario       = d.IvaUnitario
                }).ToList()
            };
        }

        // ============================
        // CANCELAR VENTA
        // ============================
        public async Task<bool> CancelarVentaAsync(int idUsuario, int idVenta)
        {
            var venta = await _db.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            if (venta is null) return false;
            if (venta.Estatus == EstatusVenta.Cancelada) return true;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var d in venta.Detalles)
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
