using BioAlga.Backend.Dtos;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly ICompraService _svc;
    private readonly IProductoService _prod; // ya existe en tu solución

    public ComprasController(ICompraService svc, IProductoService prod)
    {
        _svc = svc; _prod = prod;
    }

    // Crear borrador
    [HttpPost]
    [ProducesResponseType(typeof(CompraDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CompraDto>> Crear([FromBody] CrearCompraDto dto)
    {
        var c = await _svc.CrearBorradorAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = c.Id_Compra }, c);
    }

    // Obtener
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CompraDto>> Obtener([FromRoute] int id)
    {
        var c = await _svc.ObtenerAsync(id);
        return c is null ? NotFound() : Ok(c);
    }

    // Buscar compras
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CompraDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CompraDto>>> Buscar([FromQuery] CompraQueryParams p)
    {
        var page = await _svc.BuscarAsync(p);
        return Ok(page);
    }

    // Agregar renglón
    [HttpPost("{id:int}/renglones")]
    [ProducesResponseType(typeof(CompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CompraDto>> AgregarRenglon([FromRoute] int id, [FromBody] AgregarRenglonDto dto)
    {
        var c = await _svc.AgregarRenglonAsync(id, dto);
        return c is null ? NotFound() : Ok(c);
    }

    // Eliminar renglón
    [HttpDelete("{id:int}/renglones/{idDetalle:int}")]
    [ProducesResponseType(typeof(CompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CompraDto>> EliminarRenglon([FromRoute] int id, [FromRoute] int idDetalle)
    {
        var c = await _svc.EliminarRenglonAsync(id, idDetalle);
        return c is null ? NotFound() : Ok(c);
    }

    // Confirmar compra -> crea Entradas de inventario
    [HttpPost("{id:int}/confirmar")]
    [ProducesResponseType(typeof(ConfirmarCompraResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConfirmarCompraResponse>> Confirmar([FromRoute] int id, [FromQuery] int idUsuario)
    {
        var r = await _svc.ConfirmarAsync(id, idUsuario);
        return r is null ? NotFound() : Ok(r);
    }

    // ===========================
    // Autocomplete productos por nombre (o SKU/código)
    // ===========================
    [HttpGet("productos")]
    [ProducesResponseType(typeof(List<ProductoLookupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductoLookupDto>>> BuscarProductos([FromQuery] string q, [FromQuery] int limit = 10)
    {
        // Reusa tu IProductoService (supuesto: ya tienes búsqueda). Si no, expongo una versión mínima:
        if (string.IsNullOrWhiteSpace(q)) return Ok(new List<ProductoLookupDto>());
        var list = await _prod.BuscarBasicoAsync(q.Trim(), limit); // <= implementa en tu servicio de productos
        return Ok(list);
    }
}
