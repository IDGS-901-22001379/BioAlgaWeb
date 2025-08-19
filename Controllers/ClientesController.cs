// Controllers/ClientesController.cs
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ClientesController(ApplicationDbContext db) => _db = db;

        // ------------------------------
        // Helpers de mapeo
        // ------------------------------
        private static ClienteDto ToDto(Cliente c) => new()
        {
            Id_Cliente     = c.Id_Cliente,
            Nombre         = c.Nombre,
            Apellido       = c.Apellido,
            Correo         = c.Correo,
            Telefono       = c.Telefono,
            Direccion      = c.Direccion,
            Tipo_Cliente   = c.Tipo_Cliente,
            Estado         = c.Estado,
            Fecha_Registro = c.Fecha_Registro
        };

        // ------------------------------
        // GET: api/clientes (con filtros)
        // ------------------------------
        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] ClienteQueryParams q)
        {
            if (q.Page <= 0) q.Page = 1;
            if (q.PageSize <= 0 || q.PageSize > 100) q.PageSize = 10;

            var query = _db.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Q))
            {
                var term = q.Q.Trim();
                query = query.Where(c =>
                    (c.Nombre   ?? "").Contains(term) ||
                    (c.Apellido ?? "").Contains(term) ||
                    (c.Correo   ?? "").Contains(term) ||
                    (c.Telefono ?? "").Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(q.Estado))
                query = query.Where(c => c.Estado == q.Estado);

            if (!string.IsNullOrWhiteSpace(q.Tipo_Cliente))
                query = query.Where(c => c.Tipo_Cliente == q.Tipo_Cliente);

            if (q.Desde.HasValue)
                query = query.Where(c => c.Fecha_Registro >= q.Desde.Value);

            if (q.Hasta.HasValue)
                query = query.Where(c => c.Fecha_Registro <= q.Hasta.Value);

            var total = await query.CountAsync();

            var items = await query
    .OrderByDescending(c => c.Fecha_Registro)
    .Skip((q.Page - 1) * q.PageSize)
    .Take(q.PageSize)
    .Select(c => new ClienteDto
    {
        Id_Cliente     = c.Id_Cliente,
        Nombre         = c.Nombre,
        Apellido       = c.Apellido,
        Correo         = c.Correo,
        Telefono       = c.Telefono,
        Direccion      = c.Direccion,
        Tipo_Cliente   = c.Tipo_Cliente,
        Estado         = c.Estado,
        Fecha_Registro = c.Fecha_Registro
    })
    .ToListAsync();


            return Ok(new { total, items, page = q.Page, pageSize = q.PageSize });
        }

        // ------------------------------
        // GET: api/clientes/{id}
        // ------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var c = await _db.Clientes.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id_Cliente == id);
            if (c is null) return NotFound();
            return Ok(ToDto(c));
        }

        // ------------------------------
        // POST: api/clientes
        // ------------------------------
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Create([FromBody] ClienteCreateRequest req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Valida correo duplicado si viene
            if (!string.IsNullOrWhiteSpace(req.Correo))
            {
                var exist = await _db.Clientes.AnyAsync(c => c.Correo == req.Correo);
                if (exist) return Conflict("El correo ya está registrado para otro cliente.");
            }

            var entity = new Cliente
            {
                Nombre       = req.Nombre.Trim(),
                Apellido     = req.Apellido,
                Correo       = req.Correo,
                Telefono     = req.Telefono,
                Direccion    = req.Direccion,
                Tipo_Cliente = string.IsNullOrWhiteSpace(req.Tipo_Cliente) ? "Normal" : req.Tipo_Cliente!,
                Estado       = string.IsNullOrWhiteSpace(req.Estado) ? "Activo" : req.Estado!,
                Fecha_Registro = DateTime.UtcNow
            };

            _db.Clientes.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = entity.Id_Cliente }, ToDto(entity));
        }

        // ------------------------------
        // PUT: api/clientes/{id}
        // ------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] ClienteUpdateRequest req)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var c = await _db.Clientes.FirstOrDefaultAsync(x => x.Id_Cliente == id);
            if (c is null) return NotFound();

            // Si envían correo, valida duplicado
            if (!string.IsNullOrWhiteSpace(req.Correo))
            {
                var duplicated = await _db.Clientes
                    .AnyAsync(x => x.Id_Cliente != id && x.Correo == req.Correo);
                if (duplicated) return Conflict("El correo ya está registrado para otro cliente.");
            }

            if (!string.IsNullOrWhiteSpace(req.Nombre))      c.Nombre       = req.Nombre!.Trim();
            if (req.Apellido   != null)                      c.Apellido     = req.Apellido;
            if (req.Correo     != null)                      c.Correo       = req.Correo;
            if (req.Telefono   != null)                      c.Telefono     = req.Telefono;
            if (req.Direccion  != null)                      c.Direccion    = req.Direccion;
            if (req.Tipo_Cliente != null)                    c.Tipo_Cliente = req.Tipo_Cliente;
            if (req.Estado       != null)                    c.Estado       = req.Estado;

            await _db.SaveChangesAsync();
            return Ok(ToDto(c));
        }

        // ------------------------------
        // DELETE: api/clientes/{id}
        // ------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Clientes.FirstOrDefaultAsync(x => x.Id_Cliente == id);
            if (c is null) return NotFound();

            _db.Clientes.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
