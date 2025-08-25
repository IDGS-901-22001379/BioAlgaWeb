using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories;

public class CompraRepository : ICompraRepository
{
    private readonly ApplicationDbContext _db;
    public CompraRepository(ApplicationDbContext db) => _db = db;

    public async Task<Compra> CreateAsync(Compra compra)
    {
        _db.Compras.Add(compra);
        await _db.SaveChangesAsync();
        return compra;
    }

    public Task<Compra?> GetAsync(int id) =>
        _db.Compras
           .Include(c => c.Detalles)
           .ThenInclude(d => d.Producto)
           .FirstOrDefaultAsync(c => c.IdCompra == id);

    public async Task<(IEnumerable<Compra> items, int total)> SearchAsync(string? q, DateTime? desde, DateTime? hasta, int page, int pageSize)
    {
        var qry = _db.Compras.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            if (int.TryParse(q, out var folio))
                qry = qry.Where(c => c.IdCompra == folio);
            else
                qry = qry.Where(c => (c.ProveedorTexto != null && c.ProveedorTexto.Contains(q)) || c.ProveedorId != null);
        }

        if (desde.HasValue) qry = qry.Where(c => c.FechaCompra >= desde.Value);
        if (hasta.HasValue) qry = qry.Where(c => c.FechaCompra <= hasta.Value);

        var total = await qry.CountAsync();
        var items = await qry
            .OrderByDescending(c => c.FechaCompra)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(c => c.Detalles)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddDetalleAsync(DetalleCompra d)
    {
        _db.DetalleCompras.Add(d);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveDetalleAsync(int idDetalle)
    {
        var d = await _db.DetalleCompras.FindAsync(idDetalle);
        if (d != null)
        {
            _db.DetalleCompras.Remove(d);
            await _db.SaveChangesAsync();
        }
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
