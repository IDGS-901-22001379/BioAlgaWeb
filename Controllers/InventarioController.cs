using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public InventarioController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ----------------------------------------------------
        //  STOCK ACTUAL
        //  GET /api/inventario/stock-actual?producto_id=123
        // ----------------------------------------------------
        [HttpGet("stock-actual")]
        public async Task<ActionResult<StockActualResponse>> StockActual(
            [FromQuery(Name = "producto_id")] int productoId,
            CancellationToken ct = default)
        {
            var stock = await CalcularStockAsync(productoId, ct);

            var res = new StockActualResponse
            {
                Id_Producto = productoId,
                Stock = stock
            };
            return Ok(res);
        }

        // ----------------------------------------------------
        //  KARDEX
        //  GET /api/inventario/kardex?producto_id=123&desde=YYYY-MM-DD&hasta=YYYY-MM-DD
        // ----------------------------------------------------
        [HttpGet("kardex")]
        public async Task<ActionResult<List<KardexItemDto>>> Kardex(
            [FromQuery(Name = "producto_id")] int productoId,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            CancellationToken ct = default)
        {
            var q = _db.Set<InventarioMovimiento>()
                       .Where(m => m.IdProducto == productoId);

            if (desde.HasValue) q = q.Where(m => m.Fecha >= desde.Value);
            if (hasta.HasValue) q = q.Where(m => m.Fecha <= hasta.Value.AddDays(1).AddTicks(-1));

            var items = await q
                .OrderBy(m => m.Fecha)
                .Select(m => new KardexItemDto
                {
                    Fecha    = m.Fecha,
                    Tipo     = m.TipoMovimiento, // "Entrada" | "Salida" | "Ajuste"
                    Cantidad = m.TipoMovimiento == nameof(TipoMovimiento.Salida) ? -m.Cantidad : m.Cantidad,
                    Origen   = $"{m.OrigenTipo}{(m.OrigenId != null ? $" #{m.OrigenId}" : "")}"
                })
                .ToListAsync(ct);

            // Saldo acumulado
            int saldo = 0;
            foreach (var it in items)
            {
                saldo += it.Cantidad;
                it.Saldo = saldo;
            }

            return Ok(items);
        }

        // ----------------------------------------------------
        //  AGREGAR STOCK (Ajuste → Entrada)
        //  POST /api/inventario/agregar
        //  { id_Producto, cantidad, id_Usuario, motivo }
        // ----------------------------------------------------
        [HttpPost("agregar")]
        public async Task<ActionResult<MovimientoResultDto>> Agregar(
            [FromBody] AjusteInventarioDto dto,
            CancellationToken ct = default)
        {
            if (dto is null) return BadRequest("Payload requerido.");
            if (dto.Cantidad <= 0) return BadRequest("La cantidad debe ser mayor que cero.");

            var prod = await _db.Set<Producto>()
                                .Where(p => p.IdProducto == dto.Id_Producto)
                                .Select(p => new { p.IdProducto, p.Estatus })
                                .FirstOrDefaultAsync(ct);
            if (prod is null) return NotFound("Producto no encontrado.");
            if (string.Equals(prod.Estatus, "Inactivo", StringComparison.OrdinalIgnoreCase))
                return BadRequest("El producto está inactivo.");

            var mov = new InventarioMovimiento
            {
                IdProducto     = dto.Id_Producto,
                TipoMovimiento = nameof(TipoMovimiento.Entrada),
                Cantidad       = dto.Cantidad,
                Fecha          = DateTime.UtcNow,
                OrigenTipo     = nameof(OrigenMovimiento.Ajuste),
                OrigenId       = null,
                IdUsuario      = dto.Id_Usuario,
                Referencia     = string.IsNullOrWhiteSpace(dto.Motivo) ? null : dto.Motivo!.Trim()
            };

            await _db.Set<InventarioMovimiento>().AddAsync(mov, ct);
            await _db.SaveChangesAsync(ct);

            var stockDespues = await CalcularStockAsync(dto.Id_Producto, ct);

            return Ok(new MovimientoResultDto
            {
                Id_Movimiento  = mov.IdMovimiento,
                Id_Producto    = mov.IdProducto,
                Tipo_Movimiento= mov.TipoMovimiento,
                Cantidad       = mov.Cantidad,
                Fecha          = mov.Fecha,
                Origen_Tipo    = mov.OrigenTipo,
                Origen_Id      = mov.OrigenId,
                Referencia     = mov.Referencia,
                Stock_Despues  = stockDespues
            });
        }

        // ----------------------------------------------------
        //  QUITAR STOCK (Ajuste → Salida)
        //  POST /api/inventario/quitar
        //  { id_Producto, cantidad, id_Usuario, motivo }
        // ----------------------------------------------------
        [HttpPost("quitar")]
        public async Task<ActionResult<MovimientoResultDto>> Quitar(
            [FromBody] AjusteInventarioDto dto,
            CancellationToken ct = default)
        {
            if (dto is null) return BadRequest("Payload requerido.");
            if (dto.Cantidad <= 0) return BadRequest("La cantidad debe ser mayor que cero.");

            var existe = await _db.Set<Producto>()
                                  .AnyAsync(p => p.IdProducto == dto.Id_Producto, ct);
            if (!existe) return NotFound("Producto no encontrado.");

            var stockActual = await CalcularStockAsync(dto.Id_Producto, ct);
            if (stockActual < dto.Cantidad)
                return BadRequest($"Stock insuficiente. Actual: {stockActual}");

            var mov = new InventarioMovimiento
            {
                IdProducto     = dto.Id_Producto,
                TipoMovimiento = nameof(TipoMovimiento.Salida),
                Cantidad       = dto.Cantidad,
                Fecha          = DateTime.UtcNow,
                OrigenTipo     = nameof(OrigenMovimiento.Ajuste),
                OrigenId       = null,
                IdUsuario      = dto.Id_Usuario,
                Referencia     = string.IsNullOrWhiteSpace(dto.Motivo) ? null : dto.Motivo!.Trim()
            };

            await _db.Set<InventarioMovimiento>().AddAsync(mov, ct);
            await _db.SaveChangesAsync(ct);

            var stockDespues = await CalcularStockAsync(dto.Id_Producto, ct);

            return Ok(new MovimientoResultDto
            {
                Id_Movimiento  = mov.IdMovimiento,
                Id_Producto    = mov.IdProducto,
                Tipo_Movimiento= mov.TipoMovimiento,
                Cantidad       = mov.Cantidad,
                Fecha          = mov.Fecha,
                Origen_Tipo    = mov.OrigenTipo,
                Origen_Id      = mov.OrigenId,
                Referencia     = mov.Referencia,
                Stock_Despues  = stockDespues
            });
        }

        // ----------------------------------------------------
        //  Helper: cálculo de stock
        // ----------------------------------------------------
        private async Task<int> CalcularStockAsync(int productoId, CancellationToken ct)
        {
            return await _db.Set<InventarioMovimiento>()
                .Where(m => m.IdProducto == productoId)
                .Select(m => m.TipoMovimiento == nameof(TipoMovimiento.Salida) ? -m.Cantidad : m.Cantidad)
                .SumAsync(ct);
        }
    }
}
