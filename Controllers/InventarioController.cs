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

        // GET /api/inventario/stock-actual?producto_id=123
        [HttpGet("stock-actual")]
        public async Task<ActionResult<StockActualResponse>> StockActual(
            [FromQuery(Name = "producto_id")] int productoId,
            CancellationToken ct = default)
        {
            // Usamos Set<> por si no tienes DbSet<InventarioMovimiento> declarado
            var movs = _db.Set<InventarioMovimiento>()
                          .Where(m => m.IdProducto == productoId);

            // Salida suma en negativo; Ajuste se graba con signo según corresponda
            var stock = await movs
                .Select(m => m.TipoMovimiento == nameof(TipoMovimiento.Salida)
                             ? -m.Cantidad
                             : m.Cantidad)
                .SumAsync(ct);

            var res = new StockActualResponse
            {
                Id_Producto = productoId,
                Stock = stock
            };
            return Ok(res);
        }

        // GET /api/inventario/kardex?producto_id=123&desde=2025-08-01&hasta=2025-08-31
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
                    Fecha   = m.Fecha,
                    Tipo    = m.TipoMovimiento, // "Entrada" | "Salida" | "Ajuste"
                    Cantidad= m.TipoMovimiento == nameof(TipoMovimiento.Salida) ? -m.Cantidad : m.Cantidad,
                    Origen  = $"{m.OrigenTipo}{(m.OrigenId != null ? $" #{m.OrigenId}" : "")}"
                })
                .ToListAsync(ct);

            // Saldo acumulado (opcional)
            int saldo = 0;
            foreach (var it in items)
            {
                saldo += it.Cantidad;
                it.Saldo = saldo;
            }

            return Ok(items);
        }
    }
}
