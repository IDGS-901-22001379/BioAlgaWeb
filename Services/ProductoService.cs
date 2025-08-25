// Services/ProductoService.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using BioAlga.Backend.Data;                 // <- para _db (autocomplete)
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;        // <- para EF.Functions.Like, ToListAsync

namespace BioAlga.Backend.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;  // <- para BuscarBasicoAsync

        public ProductoService(IProductoRepository repo, IMapper mapper, ApplicationDbContext db)
        {
            _repo = repo;
            _mapper = mapper;
            _db = db;
        }

        public async Task<PagedResponse<ProductoDto>> BuscarAsync(ProductoQueryParams qp, CancellationToken ct = default)
        {
            var (items, total) = await _repo.SearchAsync(qp, ct);
            var dtos = _mapper.Map<List<ProductoDto>>(items);

            return new PagedResponse<ProductoDto>
            {
                Items = dtos,
                Total = total,
                Page = qp.Page,
                PageSize = qp.PageSize
            };
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            var prod = await _repo.GetByIdAsync(id, ct);
            return prod is null ? null : _mapper.Map<ProductoDto>(prod);
        }

        public async Task<ProductoDto> CrearAsync(CrearProductoDto dto, CancellationToken ct = default)
        {
            // SKU único
            if (await _repo.ExistsSkuAsync(dto.CodigoSku, null, ct))
                throw new InvalidOperationException("El SKU ya existe");

            var entity = _mapper.Map<Producto>(dto);
            var creado = await _repo.AddAsync(entity, ct);
            return _mapper.Map<ProductoDto>(creado);
        }

        public async Task<bool> ActualizarAsync(int id, ActualizarProductoDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return false;

            // SKU único al actualizar
            if (await _repo.ExistsSkuAsync(dto.CodigoSku, id, ct))
                throw new InvalidOperationException("El SKU ya existe");

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity, ct);
            return true;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            if (entity is null) return false;

            await _repo.DeleteAsync(entity, ct);
            return true;
        }

        /// <summary>
        /// Búsqueda rápida/autocomplete por Nombre, SKU o Código de barras.
        /// Devuelve solo productos Activos y limita el resultado.
        /// </summary>
        public async Task<List<ProductoLookupDto>> BuscarBasicoAsync(string q, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(q)) return new List<ProductoLookupDto>();
            q = q.Trim();

            // Con utf8mb4_unicode_ci la comparación ya es case-insensitive en MySQL
            var query = _db.Productos
                .Where(p => p.Estatus == "Activo" &&
                           (EF.Functions.Like(p.Nombre, $"%{q}%")
                         || EF.Functions.Like(p.CodigoSku, $"%{q}%")
                         || (p.CodigoBarras != null && EF.Functions.Like(p.CodigoBarras, $"%{q}%"))))
                .OrderBy(p => p.Nombre)
                .Take(limit)
                .Select(p => new ProductoLookupDto
                {
                    Id_Producto   = p.IdProducto,
                    Nombre        = p.Nombre,
                    SKU           = p.CodigoSku,
                    Codigo_Barras = p.CodigoBarras,
                    Estatus       = p.Estatus
                });

            return await query.ToListAsync();
        }
    }
}
