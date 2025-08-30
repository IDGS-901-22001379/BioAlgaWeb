using AutoMapper;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Services
{
    public class DevolucionService : IDevolucionService
    {
        private readonly IDevolucionRepository _repo;
        private readonly IProductoRepository _productoRepo;
        private readonly IInventarioRepository _inventarioRepo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;

        public DevolucionService(
            IDevolucionRepository repo,
            IProductoRepository productoRepo,
            IInventarioRepository inventarioRepo,
            IUsuarioRepository usuarioRepo,
            IMapper mapper,
            ApplicationDbContext db)
        {
            _repo = repo;
            _productoRepo = productoRepo;
            _inventarioRepo = inventarioRepo;
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _db = db;
        }

        public async Task<DevolucionDto> CrearAsync(int idUsuario, DevolucionCreateRequest req, CancellationToken ct = default)
        {
            // ===== Validaciones básicas =====
            if (req is null) throw new ArgumentNullException(nameof(req));
            if (string.IsNullOrWhiteSpace(req.Motivo))
                throw new ArgumentException("El motivo de la devolución es requerido.", nameof(req.Motivo));
            if (req.Lineas is null || req.Lineas.Count == 0)
                throw new ArgumentException("La devolución debe incluir al menos una línea.");

            foreach (var l in req.Lineas)
            {
                if (l.IdProducto <= 0) throw new ArgumentException("IdProducto inválido en una línea.");
                if (l.Cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor que cero.");
                if (l.ImporteLineaTotal < 0) throw new ArgumentException("El importe total de la línea no puede ser negativo.");
            }

            // ===== Usuario (para Id y snapshot de nombre) =====
            var usuario = await _usuarioRepo.GetByIdAsync(idUsuario);
            if (usuario is null)
                throw new InvalidOperationException("Usuario no válido para registrar la devolución.");

            var usuarioNombre = usuario.Nombre_Usuario ?? "Desconocido";

            // ===== Productos: validamos existencia y tomamos nombre para snapshot =====
            var idsProd = req.Lineas.Select(x => x.IdProducto).Distinct().ToList();

            var productos = await (
                from p in _db.Productos
                join id in idsProd on p.IdProducto equals id
                select new { p.IdProducto, p.Nombre }
            ).ToListAsync(ct);

            if (productos.Count != idsProd.Count)
                throw new InvalidOperationException("Uno o más productos de la devolución no existen.");

            var nombrePorProducto = productos.ToDictionary(p => p.IdProducto, p => p.Nombre);

            // ===== Mapear a entidades =====
            var entity = _mapper.Map<Devolucion>(req);
            entity.IdUsuario = idUsuario;
            entity.UsuarioNombre = usuarioNombre;

            // Llenar snapshots de productos en cada detalle
            foreach (var d in entity.Detalles)
                d.ProductoNombre = nombrePorProducto[d.IdProducto];

            // Calcular total devuelto
            entity.TotalDevuelto = entity.Detalles.Sum(x => x.ImporteLineaTotal);
            if (entity.TotalDevuelto < 0)
                throw new InvalidOperationException("El total devuelto no puede ser negativo.");

            // ===== Persistencia (transacción por seguridad) =====
            using var trx = await _db.Database.BeginTransactionAsync(ct);

            await _repo.CrearAsync(entity, ct);
            await _repo.GuardarCambiosAsync(ct);

            // ===== Inventario =====
            if (entity.RegresaInventario)
            {
                foreach (var det in entity.Detalles)
                {
                    var mov = new InventarioMovimiento
                    {
                        IdProducto = det.IdProducto,
                        TipoMovimiento = "Entrada",
                        Cantidad = det.Cantidad,
                        Fecha = DateTime.UtcNow,
                        OrigenTipo = "Devolucion",
                        OrigenId = entity.IdDevolucion,
                        IdUsuario = idUsuario,
                        Referencia = $"DEV-{entity.IdDevolucion}"
                    };

                    await _inventarioRepo.AddMovimientoAsync(mov, ct);
                }

                await _inventarioRepo.GuardarCambiosAsync(ct);
            }

            await trx.CommitAsync(ct);

            // ===== Salida DTO =====
            var guardada = await _repo.ObtenerPorIdAsync(entity.IdDevolucion, ct);
            if (guardada is null)
                throw new InvalidOperationException("No fue posible recuperar la devolución recién creada.");

            return _mapper.Map<DevolucionDto>(guardada);
        }

        public async Task<DevolucionDto?> ObtenerAsync(int idDevolucion, CancellationToken ct = default)
        {
            var dev = await _repo.ObtenerPorIdAsync(idDevolucion, ct);
            return dev is null ? null : _mapper.Map<DevolucionDto>(dev);
        }

        public async Task<PagedResponse<DevolucionDto>> BuscarAsync(DevolucionQueryParams qp, CancellationToken ct = default)
        {
            var (items, total) = await _repo.BuscarAsync(qp, ct);
            var list = _mapper.Map<List<DevolucionDto>>(items);

            return new PagedResponse<DevolucionDto>
            {
                Items = list,
                Total = total,
                Page = qp.Page <= 0 ? 1 : qp.Page,
                PageSize = qp.PageSize <= 0 ? 10 : qp.PageSize
            };
        }
    }
}
