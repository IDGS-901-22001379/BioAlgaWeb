// Controllers/UsuariosController.cs
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public UsuariosController(ApplicationDbContext db) => _db = db;

        // =========================================
        // Helpers
        // =========================================
        private static UsuarioDto MapToDto(Usuario u) => new()
        {
            Id_Usuario = u.Id_Usuario,
            Nombre_Usuario = u.Nombre_Usuario,
            Rol = u.Rol != null ? u.Rol.Nombre : string.Empty,
            Id_Rol = u.Id_Rol,
            Id_Empleado = u.Id_Empleado,
            Nombre_Empleado = u.Empleado?.Nombre,
            Apellido_Paterno = u.Empleado?.Apellido_Paterno,
            Apellido_Materno = u.Empleado?.Apellido_Materno,
            Activo = u.Activo,
            Ultimo_Login = u.Ultimo_Login,
            Fecha_Registro = u.Fecha_Registro
        };

        // =========================================
        // GET: api/usuarios
        // Lista completa (sin paginar)
        // =========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            var usuarios = await _db.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .Select(u => MapToDto(u))
                .ToListAsync();

            return Ok(usuarios);
        }

        // =========================================
        // GET: api/usuarios/buscar
        // Buscar + paginación + filtro activo
        // =========================================
        [HttpGet("buscar")]
        public async Task<ActionResult<object>> Buscar(
            [FromQuery] string? nombre,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? activo = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 10;

            var q = _db.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                var n = nombre.Trim();
                q = q.Where(u =>
                    u.Nombre_Usuario.Contains(n) ||
                    (u.Empleado != null &&
                        (
                            (u.Empleado.Nombre ?? "").Contains(n) ||
                            (u.Empleado.Apellido_Paterno ?? "").Contains(n) ||
                            (u.Empleado.Apellido_Materno ?? "").Contains(n)
                        )
                    ));
            }

            if (activo.HasValue)
                q = q.Where(u => u.Activo == activo.Value);

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(u => u.Nombre_Usuario)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => MapToDto(u))
                .ToListAsync();

            return Ok(new { total, items, page, pageSize });
        }

        // =========================================
        // GET: api/usuarios/{id}
        // =========================================
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            var u = await _db.Usuarios
                .AsNoTracking()
                .Include(x => x.Rol)
                .Include(x => x.Empleado)
                .FirstOrDefaultAsync(x => x.Id_Usuario == id);

            if (u is null) return NotFound();

            return Ok(MapToDto(u));
        }

        // =========================================
        // POST: api/usuarios
        // Crear usuario con BCrypt
        // =========================================
        public class UsuarioCreateRequest
        {
            public string Nombre_Usuario { get; set; } = string.Empty;
            public string Contrasena { get; set; } = string.Empty;
            public int Id_Rol { get; set; }
            public int? Id_Empleado { get; set; }
            public bool Activo { get; set; } = true;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> Create([FromBody] UsuarioCreateRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Nombre_Usuario) || string.IsNullOrWhiteSpace(req.Contrasena))
                return BadRequest("Nombre de usuario y contraseña son obligatorios.");

            var exists = await _db.Usuarios.AnyAsync(u => u.Nombre_Usuario == req.Nombre_Usuario);
            if (exists) return Conflict("El nombre de usuario ya existe.");

            var nuevo = new Usuario
            {
                Nombre_Usuario = req.Nombre_Usuario,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(req.Contrasena), // ✅ Encriptar
                Id_Rol = req.Id_Rol,
                Id_Empleado = req.Id_Empleado,
                Activo = req.Activo,
                Fecha_Registro = DateTime.UtcNow
            };

            _db.Usuarios.Add(nuevo);
            await _db.SaveChangesAsync();

            var creado = await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .FirstAsync(u => u.Id_Usuario == nuevo.Id_Usuario);

            return CreatedAtAction(nameof(GetById), new { id = creado.Id_Usuario }, MapToDto(creado));
        }

        // =========================================
        // PUT: api/usuarios/{id}
        // Actualizar datos con opción de re-hashear contraseña
        // =========================================
        public class UsuarioUpdateRequest
        {
            public string? Nombre_Usuario { get; set; }
            public string? Contrasena { get; set; }
            public int? Id_Rol { get; set; }
            public int? Id_Empleado { get; set; }
            public bool? Activo { get; set; }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UsuarioDto>> Update(int id, [FromBody] UsuarioUpdateRequest req)
        {
            var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id_Usuario == id);
            if (u is null) return NotFound();

            if (!string.IsNullOrWhiteSpace(req.Nombre_Usuario) && req.Nombre_Usuario != u.Nombre_Usuario)
            {
                var exists = await _db.Usuarios.AnyAsync(x => x.Nombre_Usuario == req.Nombre_Usuario && x.Id_Usuario != id);
                if (exists) return Conflict("El nombre de usuario ya está en uso.");
                u.Nombre_Usuario = req.Nombre_Usuario!;
            }

            if (!string.IsNullOrWhiteSpace(req.Contrasena))
                u.Contrasena = BCrypt.Net.BCrypt.HashPassword(req.Contrasena); // ✅ Re-hash

            if (req.Id_Rol.HasValue) u.Id_Rol = req.Id_Rol.Value;
            if (req.Id_Empleado.HasValue) u.Id_Empleado = req.Id_Empleado.Value == 0 ? null : req.Id_Empleado;
            if (req.Activo.HasValue) u.Activo = req.Activo.Value;

            await _db.SaveChangesAsync();

            var actualizado = await _db.Usuarios
                .AsNoTracking()
                .Include(x => x.Rol)
                .Include(x => x.Empleado)
                .FirstAsync(x => x.Id_Usuario == id);

            return Ok(MapToDto(actualizado));
        }

        // =========================================
        // DELETE: api/usuarios/{id}
        // =========================================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Id_Usuario == id);
            if (u is null) return NotFound();

            _db.Usuarios.Remove(u);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
