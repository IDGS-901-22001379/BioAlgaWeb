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

            // Ordenamiento básico
            bool desc = (sortDir ?? "DESC").Equals("DESC", StringComparison.OrdinalIgnoreCase);
            query = (sortBy ?? "FechaPedido") switch
            {
                "Total"        => (desc ? query.OrderByDescending(x => x.Total)       : query.OrderBy(x => x.Total)),
                "Estatus"      => (desc ? query.OrderByDescending(x => x.Estatus)     : query.OrderBy(x => x.Estatus)),
                "FechaPedido"  => (desc ? query.OrderByDescending(x => x.FechaPedido) : query.OrderBy(x => x.FechaPedido)),
                _              => (desc ? query.OrderByDescending(x => x.FechaPedido) : query.OrderBy(x => x.FechaPedido)),
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
            // Validaciones
            await ValidarClienteExiste(req.IdCliente);

            if (req.Lineas is null || req.Lineas.Count == 0)
                throw new ValidationException("El pedido debe tener al menos una línea.");

            // Construcción
            var pedido = _mapper.Map<Pedido>(req);
            pedido.IdUsuario = idUsuario;
            pedido.FechaPedido = DateTime.Now;
            pedido.Estatus = EstatusPedido.Borrador;

            // Cargar líneas (sin precio fijo aún)
            foreach (var l in req.Lineas)
            {
                var det = _mapper.Map<DetallePedido>(l);
                det.PrecioUnitario = l.PrecioUnitarioOverride ?? 0m; // se congelará en Confirmar si 0
                pedido.Detalles.Add(det);
            }

            RecalcularTotales(pedido); // usa el precio que tenga la línea (si 0, subtotal quedará 0 hasta confirmar)
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

            // Cambios permitidos
            if (req.IdCliente.HasValue)
            {
                await ValidarClienteExiste(req.IdCliente.Value);
                pedido.IdCliente = req.IdCliente.Value;
            }

            if (req.FechaRequerida.HasValue) pedido.FechaRequerida = req.FechaRequerida.Value;
            if (req.Anticipo.HasValue) pedido.Anticipo = req.Anticipo.Value;
            if (req.Notas != null) pedido.Notas = req.Notas;

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

            // Reemplazo total
            pedido.Detalles.Clear();
            foreach (var l in req.Lineas)
            {
                var det = _mapper.Map<DetallePedido>(l);
                det.PrecioUnitario = l.PrecioUnitarioOverride ?? 0m; // se congelará en Confirmar si 0
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
                // Editar existente
                var det = pedido.Detalles.FirstOrDefault(x => x.IdDetalle == req.IdDetalle.Value)
                    ?? throw new KeyNotFoundException("Detalle no encontrado.");

                det.IdProducto = req.IdProducto;
                det.Cantidad   = req.Cantidad;
                if (req.PrecioUnitarioOverride.HasValue)
                    det.PrecioUnitario = req.PrecioUnitarioOverride.Value;
            }
            else
            {
                // Agregar nueva
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
            // Confirmación: congela precios por tipo de cliente, opcional reserva
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (pedido.Estatus != EstatusPedido.Borrador)
                throw new ValidationException("Solo se puede confirmar un pedido en Borrador.");

            // Trae tipo de cliente
            var cliente = await _db.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCliente == pedido.IdCliente)
                ?? throw new ValidationException("Cliente no existe.");

            // Congelar precios
            foreach (var d in pedido.Detalles)
            {
                if (d.PrecioUnitario > 0) continue; // ya forzado por override

                var precio = await ObtenerPrecioVigentePorClienteAsync(d.IdProducto, cliente.TipoCliente);
                if (precio is null)
                    throw new ValidationException($"No existe precio vigente para el producto {d.IdProducto} (tipo: {cliente.TipoCliente}).");

                d.PrecioUnitario = precio.Value;
            }

            pedido.Estatus = EstatusPedido.Confirmado;
            RecalcularTotales(pedido);

            // TODO: Reserva de stock si req.ReservarStock == true
            // Política recomendada: marcar "reservado" lógico; movimientos reales al facturar.

            await _db.SaveChangesAsync();
            return await GetOrThrowDto(pedido.IdPedido);
        }

        public async Task<PedidoDto> CambiarEstatusAsync(int idUsuario, PedidoCambioEstatusRequest req)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == req.IdPedido)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            // Validar transición simple (más reglas se pueden agregar)
            if (!TransicionValida(pedido.Estatus, req.NuevoEstatus))
                throw new ValidationException($"Transición no permitida: {pedido.Estatus} → {req.NuevoEstatus}");

            pedido.Estatus = req.NuevoEstatus;
            await _db.SaveChangesAsync();

            return await GetOrThrowDto(pedido.IdPedido);
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
            // IVA configurable: por simplicidad aplicamos IVA global al subtotal.
            // Si manejas exentos por producto, aquí recorrer y acumular por línea.
            var subtotal = p.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            var iva = Math.Round(subtotal * 0.16m, 2); // TODO: parametrizar
            var total = subtotal + iva;

            p.Subtotal = Math.Round(subtotal, 2);
            p.Impuestos = iva;
            p.Total = Math.Round(total, 2);
        }

        private static bool TransicionValida(EstatusPedido actual, EstatusPedido nuevo)
        {
            return (actual, nuevo) switch
            {
                (EstatusPedido.Borrador, EstatusPedido.Confirmado) => true,
                (EstatusPedido.Confirmado, EstatusPedido.Preparacion) => true,
                (EstatusPedido.Preparacion, EstatusPedido.Listo) => true,
                (EstatusPedido.Listo, EstatusPedido.Facturado) => true,
                (EstatusPedido.Facturado, EstatusPedido.Entregado) => true,
                // cancelación permitida si no está totalmente entregado
                (EstatusPedido.Borrador, EstatusPedido.Cancelado) => true,
                (EstatusPedido.Confirmado, EstatusPedido.Cancelado) => true,
                (EstatusPedido.Preparacion, EstatusPedido.Cancelado) => true,
                (EstatusPedido.Listo, EstatusPedido.Cancelado) => true,
                _ => false
            };
        }

        private async Task ValidarClienteExiste(int idCliente)
        {
            var ok = await _db.Clientes.AnyAsync(c => c.IdCliente == idCliente);
            if (!ok) throw new ValidationException("El cliente especificado no existe.");
        }

        /// <summary>
        /// Devuelve el precio vigente según el tipo de cliente.
        /// Si no hay precio del tipo, puedes cambiar política a fallback = Normal.
        /// </summary>
        private async Task<decimal?> ObtenerPrecioVigentePorClienteAsync(int idProducto, string tipoCliente)
        {
            // tipoCliente debe ser uno de: "Normal","Mayoreo","Descuento","Especial"
            // Se asume tabla producto_precios con campos: tipo_precio, precio, activo, vigente_desde, vigente_hasta
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

            // Política opcional: si no hay precio de ese tipo, fallback a Normal
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
