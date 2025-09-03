using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Models.Enums;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public PedidoService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // =========================
        // Lectura
        // =========================
        public async Task<PedidoDto?> GetAsync(int idPedido)
        {
            var query = _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .Where(p => p.IdPedido == idPedido);

            var entity = await query.FirstOrDefaultAsync();
            return entity is null ? null : _mapper.Map<PedidoDto>(entity);
        }

        public async Task<PagedResponse<PedidoListItemDto>> BuscarAsync(
            string? q, EstatusPedido? estatus, int page = 1, int pageSize = 10,
            string? sortBy = "FechaPedido", string? sortDir = "DESC")
        {
            var query = _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Cliente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qLike = q.Trim().ToLower();
                query = query.Where(p =>
                    (p.Cliente != null &&
                        ((p.Cliente.Nombre ?? "").ToLower().Contains(qLike) ||
                         (p.Cliente.ApellidoPaterno ?? "").ToLower().Contains(qLike) ||
                         (p.Cliente.ApellidoMaterno ?? "").ToLower().Contains(qLike))) ||
                    p.IdPedido.ToString() == qLike);
            }

            if (estatus.HasValue)
                query = query.Where(p => p.Estatus == estatus.Value);

            bool desc = (sortDir ?? "DESC").Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = (sortBy ?? "FechaPedido") switch
            {
                "Total"       => (desc ? query.OrderByDescending(x => x.Total)      : query.OrderBy(x => x.Total)),
                "Estatus"     => (desc ? query.OrderByDescending(x => x.Estatus)    : query.OrderBy(x => x.Estatus)),
                "FechaPedido" => (desc ? query.OrderByDescending(x => x.FechaPedido): query.OrderBy(x => x.FechaPedido)),
                _             => (desc ? query.OrderByDescending(x => x.FechaPedido): query.OrderBy(x => x.FechaPedido)),
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<PedidoListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponse<PedidoListItemDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }

        // =========================
        // Escritura: Crear/Editar
        // =========================
        public async Task<PedidoDto> CrearAsync(int idUsuario, PedidoCreateRequest req)
        {
            await ValidarClienteExiste(req.IdCliente);
            if (req.Lineas is null || req.Lineas.Count == 0)
                throw new ValidationException("El pedido debe tener al menos una línea.");

            var pedido = _mapper.Map<Pedido>(req);
            pedido.IdUsuario = idUsuario;
            pedido.FechaPedido = DateTime.Now;
            pedido.Estatus = EstatusPedido.Borrador;

            foreach (var l in req.Lineas)
            {
                var det = _mapper.Map<DetallePedido>(l);
                det.PrecioUnitario = l.PrecioUnitarioOverride ?? 0m; // se congelará al Confirmar si 0
                pedido.Detalles.Add(det);
            }

            RecalcularTotales(pedido);
            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

            return await GetOrThrowDto(pedido.IdPedido);
        }

        public async Task<PedidoDto> UpdateHeaderAsync(int idUsuario, PedidoUpdateHeaderRequest req)
        {
            var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (pedido.Estatus != EstatusPedido.Borrador)
                throw new ValidationException("Solo se puede editar la cabecera en Borrador.");

            if (req.IdCliente.HasValue)
            {
                await ValidarClienteExiste(req.IdCliente.Value);
                pedido.IdCliente = req.IdCliente.Value;
            }
            if (req.FechaRequerida.HasValue) pedido.FechaRequerida = req.FechaRequerida.Value;
            if (req.Anticipo.HasValue)       pedido.Anticipo      = req.Anticipo.Value;
            if (req.Notas != null)           pedido.Notas         = req.Notas;

            RecalcularTotales(pedido);
            await _db.SaveChangesAsync();

            return await GetOrThrowDto(pedido.IdPedido);
        }

        public async Task<PedidoDto> ReplaceLinesAsync(int idUsuario, PedidoReplaceLinesRequest req)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (pedido.Estatus != EstatusPedido.Borrador)
                throw new ValidationException("Solo se pueden reemplazar líneas en Borrador.");

            if (req.Lineas is null || req.Lineas.Count == 0)
                throw new ValidationException("Debes enviar al menos una línea.");

            pedido.Detalles.Clear();
            foreach (var l in req.Lineas)
            {
                var det = _mapper.Map<DetallePedido>(l);
                det.PrecioUnitario = l.PrecioUnitarioOverride ?? 0m;
                pedido.Detalles.Add(det);
            }

            RecalcularTotales(pedido);
            await _db.SaveChangesAsync();

            return await GetOrThrowDto(pedido.IdPedido);
        }

        public async Task<PedidoDto> EditLineAsync(int idUsuario, PedidoLineaEditRequest req)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (pedido.Estatus != EstatusPedido.Borrador)
                throw new ValidationException("Solo se pueden editar líneas en Borrador.");

            if (req.IdDetalle.HasValue)
            {
                var det = pedido.Detalles.FirstOrDefault(x => x.IdDetalle == req.IdDetalle.Value)
                    ?? throw new KeyNotFoundException("Detalle no encontrado.");

                det.IdProducto = req.IdProducto;
                det.Cantidad   = req.Cantidad;

                if (req.PrecioUnitarioOverride.HasValue)
                    det.PrecioUnitario = req.PrecioUnitarioOverride.Value;
            }
            else
            {
                var det = _mapper.Map<DetallePedido>(req);
                det.PrecioUnitario = req.PrecioUnitarioOverride ?? 0m;
                pedido.Detalles.Add(det);
            }

            RecalcularTotales(pedido);
            await _db.SaveChangesAsync();

            return await GetOrThrowDto(pedido.IdPedido);
        }

        // =========================
        // Flujo: Confirmar / Estatus
        // =========================
        public async Task<PedidoDto> ConfirmarAsync(int idUsuario, PedidoConfirmarRequest req)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (pedido.Estatus != EstatusPedido.Borrador)
                throw new ValidationException("Solo se puede confirmar un pedido en Borrador.");

            var cliente = await _db.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCliente == pedido.IdCliente)
                ?? throw new ValidationException("Cliente no existe.");

            // Congelar precios evitando updates innecesarios (previene DbUpdateConcurrencyException)
            foreach (var d in pedido.Detalles)
            {
                var entry = _db.Entry(d);
                var original = entry.Property(x => x.PrecioUnitario).OriginalValue;

                if (d.PrecioUnitario <= 0m)
                {
                    var precio = await ObtenerPrecioVigentePorClienteAsync(d.IdProducto, cliente.TipoCliente)
                                 ?? throw new ValidationException($"No existe precio vigente para el producto {d.IdProducto} ({cliente.TipoCliente}).");

                    if (original != precio)
                        d.PrecioUnitario = precio;
                    else
                        entry.Property(x => x.PrecioUnitario).IsModified = false;
                }
                else
                {
                    if (original == d.PrecioUnitario)
                        entry.Property(x => x.PrecioUnitario).IsModified = false;
                }
            }

            pedido.Estatus = EstatusPedido.Confirmado;
            RecalcularTotales(pedido);

            // TODO: si req.ReservarStock => marca reserva lógica aquí.

            await _db.SaveChangesAsync();
            return await GetOrThrowDto(pedido.IdPedido);
        }

        public async Task<PedidoDto> CambiarEstatusAsync(int idUsuario, PedidoCambioEstatusRequest req)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (!TransicionValida(pedido.Estatus, req.NuevoEstatus))
                throw new ValidationException($"Transición no permitida: {pedido.Estatus} → {req.NuevoEstatus}");

            pedido.Estatus = req.NuevoEstatus;
            await _db.SaveChangesAsync();

            return await GetOrThrowDto(pedido.IdPedido);
        }

        // =========================
        // Eliminar (Borrador / Cancelado)
        // =========================
        public async Task EliminarAsync(int idPedido)
        {
            var ped = await _db.Pedidos
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (ped.Estatus != EstatusPedido.Borrador && ped.Estatus != EstatusPedido.Cancelado)
                throw new ValidationException("Solo se pueden eliminar pedidos en Borrador o Cancelado.");

            // Cascade: elimina detalles y cabecera
            _db.Pedidos.Remove(ped);
            await _db.SaveChangesAsync();
        }

        // =========================
        // Helpers internos
        // =========================
        private async Task<PedidoDto> GetOrThrowDto(int idPedido)
        {
            var dto = await GetAsync(idPedido);
            return dto ?? throw new KeyNotFoundException("Pedido no encontrado tras operación.");
        }

        private static void RecalcularTotales(Pedido p)
        {
            var subtotal = p.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var iva = Math.Round(subtotal * 0.16m, 2); // parametriza si lo necesitas
            var total = subtotal + iva;

            p.Subtotal = Math.Round(subtotal, 2);
            p.Impuestos = iva;
            p.Total = Math.Round(total, 2);
        }

        private static bool TransicionValida(EstatusPedido actual, EstatusPedido nuevo) =>
            (actual, nuevo) switch
            {
                (EstatusPedido.Borrador,     EstatusPedido.Confirmado)  => true,
                (EstatusPedido.Confirmado,   EstatusPedido.Preparacion) => true,
                (EstatusPedido.Preparacion,  EstatusPedido.Listo)       => true,
                (EstatusPedido.Listo,        EstatusPedido.Facturado)   => true,
                (EstatusPedido.Facturado,    EstatusPedido.Entregado)   => true,

                (EstatusPedido.Borrador,     EstatusPedido.Cancelado)   => true,
                (EstatusPedido.Confirmado,   EstatusPedido.Cancelado)   => true,
                (EstatusPedido.Preparacion,  EstatusPedido.Cancelado)   => true,
                (EstatusPedido.Listo,        EstatusPedido.Cancelado)   => true,
                _ => false
            };

        private async Task ValidarClienteExiste(int idCliente)
        {
            var ok = await _db.Clientes.AnyAsync(c => c.IdCliente == idCliente);
            if (!ok) throw new ValidationException("El cliente especificado no existe.");
        }

        private async Task<decimal?> ObtenerPrecioVigentePorClienteAsync(int idProducto, string tipoCliente)
        {
            var now = DateTime.Now;

            var precio = await _db.ProductoPrecios
                .Where(pp => pp.IdProducto == idProducto
                             && pp.TipoPrecio == tipoCliente
                             && pp.Activo
                             && (pp.VigenteHasta == null || pp.VigenteHasta >= now)
                             && pp.VigenteDesde <= now)
                .OrderByDescending(pp => pp.VigenteDesde)
                .Select(pp => (decimal?)pp.Precio)
                .FirstOrDefaultAsync();

            if (precio is null && !tipoCliente.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                precio = await _db.ProductoPrecios
                    .Where(pp => pp.IdProducto == idProducto
                                 && pp.TipoPrecio == "Normal"
                                 && pp.Activo
                                 && (pp.VigenteHasta == null || pp.VigenteHasta >= now)
                                 && pp.VigenteDesde <= now)
                    .OrderByDescending(pp => pp.VigenteDesde)
                    .Select(pp => (decimal?)pp.Precio)
                    .FirstOrDefaultAsync();
            }

            return precio;
        }
    }
}
