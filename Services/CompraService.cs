using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Services;

public class CompraService : ICompraService
{
    private readonly ICompraRepository _repo;
    private readonly IInventarioRepository _inv;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _db;

    public CompraService(ICompraRepository repo, IInventarioRepository inv, IMapper mapper, ApplicationDbContext db)
    {
        _repo = repo; _inv = inv; _mapper = mapper; _db = db;
    }

    public async Task<CompraDto> CrearBorradorAsync(CrearCompraDto dto)
    {
        var compra = new Compra
        {
            ProveedorId = dto.Proveedor_Id,
            ProveedorTexto = dto.Proveedor_Texto,
            Notas = dto.Notas,
            IdUsuario = dto.Id_Usuario,
            FechaCompra = DateTime.UtcNow
        };

        await _repo.CreateAsync(compra);
        var c = await _repo.GetAsync(compra.IdCompra) ?? compra;
        var result = _mapper.Map<CompraDto>(c);
        return result;
    }

    public async Task<CompraDto?> ObtenerAsync(int id)
    {
        var c = await _repo.GetAsync(id);
        return c == null ? null : _mapper.Map<CompraDto>(c);
    }

    public async Task<PagedResponse<CompraDto>> BuscarAsync(CompraQueryParams p)
    {
        var (items, total) = await _repo.SearchAsync(p.Q, p.Desde, p.Hasta, p.Page, p.PageSize);
        return new PagedResponse<CompraDto>
        {
            Page = p.Page,
            PageSize = p.PageSize,
            Total = total,
            Items = items.Select(_mapper.Map<CompraDto>).ToList()
        };
    }

    public async Task<CompraDto?> AgregarRenglonAsync(int idCompra, AgregarRenglonDto dto)
    {
        var compra = await _repo.GetAsync(idCompra);
        if (compra == null) return null;

        // Regla: no agregar si (hipotéticamente) ya confirmada
        // (Confirmada se gestiona por endpoint; aquí prevenimos por seguridad)
        if (compra.Detalles.Count > 0 && compra.Detalles.All(d => d.IdDetalle < 0)) return null;

        var detalle = new DetalleCompra
        {
            IdCompra = idCompra,
            IdProducto = dto.Id_Producto,
            Cantidad = dto.Cantidad,
            CostoUnitario = dto.Costo_Unitario,
            IvaUnitario = dto.Iva_Unitario
        };
        await _repo.AddDetalleAsync(detalle);

        // Recalcular totales
        var c = await _repo.GetAsync(idCompra);
        if (c != null)
        {
            c.Subtotal = c.Detalles.Sum(d => d.CostoUnitario * d.Cantidad);
            c.Impuestos = c.Detalles.Sum(d => d.IvaUnitario * d.Cantidad);
            c.Total = c.Subtotal + c.Impuestos;
            await _repo.SaveAsync();
            return _mapper.Map<CompraDto>(c);
        }
        return null;
    }

    public async Task<CompraDto?> EliminarRenglonAsync(int idCompra, int idDetalle)
    {
        await _repo.RemoveDetalleAsync(idDetalle);
        var c = await _repo.GetAsync(idCompra);
        if (c == null) return null;

        c.Subtotal = c.Detalles.Sum(d => d.CostoUnitario * d.Cantidad);
        c.Impuestos = c.Detalles.Sum(d => d.IvaUnitario * d.Cantidad);
        c.Total = c.Subtotal + c.Impuestos;
        await _repo.SaveAsync();

        return _mapper.Map<CompraDto>(c);
    }

    public async Task<ConfirmarCompraResponse?> ConfirmarAsync(int idCompra, int idUsuario)
    {
        var compra = await _repo.GetAsync(idCompra);
        if (compra == null) return null;
        if (!compra.Detalles.Any()) throw new InvalidOperationException("No se puede confirmar una compra sin renglones.");

        // Generar Entradas de inventario (una por renglón)
        int movs = 0;
        foreach (var d in compra.Detalles)
        {
            var mov = new InventarioMovimiento
            {
                IdProducto = d.IdProducto,
                TipoMovimiento = nameof(Models.TipoMovimiento.Entrada),
                Cantidad = d.Cantidad,
                OrigenTipo = nameof(Models.OrigenMovimiento.Compra),
                OrigenId = idCompra,
                IdUsuario = idUsuario,
                Fecha = DateTime.UtcNow,
                Referencia  = $"Compra {idCompra} - {d.Cantidad} @ {d.CostoUnitario}"
            };
            await _inv.AddMovimientoAsync(mov);
            movs++;
        }

        // Recalcular totales por seguridad
        compra.Subtotal = compra.Detalles.Sum(d => d.CostoUnitario * d.Cantidad);
        compra.Impuestos = compra.Detalles.Sum(d => d.IvaUnitario * d.Cantidad);
        compra.Total = compra.Subtotal + compra.Impuestos;
        await _repo.SaveAsync();

        return new ConfirmarCompraResponse
        {
            Id_Compra = compra.IdCompra,
            MovimientosCreados = movs,
            Subtotal = compra.Subtotal,
            Impuestos = compra.Impuestos,
            Total = compra.Total
        };
    }
}
