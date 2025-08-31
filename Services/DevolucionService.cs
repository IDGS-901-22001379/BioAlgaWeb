using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// Enums de ventas (estatus pagada, etc.)
using BioAlga.Backend.Models.Enums;

// Alias para evitar ambigüedad con los enums de InventarioMovimiento
using OrigenMovInv = BioAlga.Backend.Models.OrigenMovimiento;
using TipoMovInv   = BioAlga.Backend.Models.TipoMovimiento;

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

        // =============================
        // Crear devolución
        // =============================
        public async Task<DevolucionDto> RegistrarDevolucionAsync(int idUsuario, DevolucionCreateRequest req)
        {
            // 1) Validar venta (si viene)
            if (req.VentaId is not null)
            {
                var ventaOk = await _db.Ventas
                    .AnyAsync(v => v.IdVenta == req.VentaId && v.Estatus == EstatusVenta.Pagada);
                if (!ventaOk)
                    throw new ValidationException("La venta no existe o no está pagada.");
            }

            // 2) Validar líneas (materializando colecciones primitivas)
            var idsDetalleVenta = req.Lineas
                .Where(l => l.IdDetalleVenta.HasValue)
                .Select(l => l.IdDetalleVenta!.Value)
                .Distinct()
                .ToList();

            var renglonesVenta = idsDetalleVenta.Count == 0
                ? new List<DetalleVenta>()
                : await _db.DetalleVentas
                    .Where(dv => idsDetalleVenta.Contains(dv.IdDetalle))
                    .ToListAsync();

            foreach (var linea in req.Lineas)
            {
                if (linea.IdDetalleVenta is not null)
                {
                    var dv = renglonesVenta.FirstOrDefault(d => d.IdDetalle == linea.IdDetalleVenta)
                        ?? throw new ValidationException("Detalle de venta no válido.");

                    if (linea.Cantidad <= 0 || linea.Cantidad > dv.Cantidad)
                        throw new ValidationException("Cantidad a devolver inválida.");
                }
                else
                {
                    if (linea.PrecioUnitario is null)
                        throw new ValidationException("Debe indicar precio si no se liga a un detalle de venta.");
                }
            }

            // 3) Crear cabecera
            var devolucion = _mapper.Map<Devolucion>(req);
            devolucion.IdUsuario = idUsuario;
            devolucion.FechaDevolucion = DateTime.Now;

            _db.Devoluciones.Add(devolucion);
            await _db.SaveChangesAsync();

            // 4) Detalle + inventario
            decimal total = 0m;

            foreach (var linea in req.Lineas)
            {
                decimal precioUnit;

                if (linea.IdDetalleVenta is not null)
                {
                    var dv = renglonesVenta.First(d => d.IdDetalle == linea.IdDetalleVenta);
                    // Ajusta si tu IVA/Descuento van incluidos o no
                    precioUnit = dv.PrecioUnitario - dv.DescuentoUnitario + dv.IvaUnitario;
                }
                else
                {
                    precioUnit = linea.PrecioUnitario!.Value;
                }

                var importe = Math.Round(precioUnit * linea.Cantidad, 2);

                var detalle = _mapper.Map<DetalleDevolucion>(linea);
                detalle.IdDevolucion = devolucion.IdDevolucion;
                detalle.ImporteLineaTotal = importe;

                _db.DetalleDevoluciones.Add(detalle);

                total += importe;

                if (req.RegresaInventario)
                {
                    _db.InventarioMovimientos.Add(new InventarioMovimiento
                    {
                        IdProducto    = linea.IdProducto,
                        // La entidad guarda string → usamos nameof para consistencia
                        TipoMovimiento = nameof(TipoMovInv.Entrada),
                        Cantidad       = linea.Cantidad,
                        OrigenTipo     = nameof(OrigenMovInv.Devolucion),
                        OrigenId       = devolucion.IdDevolucion,
                        IdUsuario      = idUsuario,
                        Referencia     = $"DEV-{devolucion.IdDevolucion}"
                    });
                }
            }

            devolucion.TotalDevuelto = Math.Round(total, 2);
            await _db.SaveChangesAsync();

            // Reconsulta para mapear DTO con detalles
            var dto = await _db.Devoluciones
                .Where(d => d.IdDevolucion == devolucion.IdDevolucion)
                .ProjectTo<DevolucionDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return dto;
        }

        // =============================
        // Obtener devolución por Id
        // =============================
        public async Task<DevolucionDto?> ObtenerPorIdAsync(int idDevolucion)
        {
            return await _db.Devoluciones
                .Where(d => d.IdDevolucion == idDevolucion)
                .ProjectTo<DevolucionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        // =============================
        // Listar devoluciones (con filtro)
        // =============================
        public async Task<List<DevolucionDto>> ListarAsync(DevolucionQueryParams filtro)
        {
            var query = _db.Devoluciones.AsQueryable();

            if (filtro.Desde.HasValue)
                query = query.Where(d => d.FechaDevolucion >= filtro.Desde.Value);

            if (filtro.Hasta.HasValue)
                query = query.Where(d => d.FechaDevolucion <= filtro.Hasta.Value);

            if (!string.IsNullOrWhiteSpace(filtro.Q))
                query = query.Where(d =>
                    d.UsuarioNombre.Contains(filtro.Q) ||
                    d.Motivo.Contains(filtro.Q) ||
                    (d.ReferenciaVenta != null && d.ReferenciaVenta.Contains(filtro.Q)));

            if (filtro.RegresaInventario.HasValue)
                query = query.Where(d => d.RegresaInventario == filtro.RegresaInventario.Value);

            return await query
                .OrderByDescending(d => d.FechaDevolucion)
                .ProjectTo<DevolucionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
