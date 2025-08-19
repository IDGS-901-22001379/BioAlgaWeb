// Services/ClienteService.cs
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services.Interfaces;

namespace BioAlga.Backend.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;

        public ClienteService(IClienteRepository repo)
        {
            _repo = repo;
        }

        // ----------------------------
        // Helpers de mapeo
        // ----------------------------
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

        private static void ApplyUpdate(Cliente entity, ClienteUpdateRequest req)
        {
            if (!string.IsNullOrWhiteSpace(req.Nombre))      entity.Nombre       = req.Nombre.Trim();
            if (req.Apellido   != null)                      entity.Apellido     = req.Apellido;
            if (req.Correo     != null)                      entity.Correo       = req.Correo;
            if (req.Telefono   != null)                      entity.Telefono     = req.Telefono;
            if (req.Direccion  != null)                      entity.Direccion    = req.Direccion;
            if (req.Tipo_Cliente != null)                    entity.Tipo_Cliente = req.Tipo_Cliente;
            if (req.Estado       != null)                    entity.Estado       = req.Estado;
        }

        // ----------------------------
        // Búsqueda + paginación
        // ----------------------------
        public async Task<(IEnumerable<ClienteDto> items, int total, int page, int pageSize)> 
            BuscarAsync(ClienteQueryParams q)
        {
            if (q.Page <= 0) q.Page = 1;
            if (q.PageSize <= 0 || q.PageSize > 100) q.PageSize = 10;

            var total = await _repo.CountAsync(q.Q, q.Estado, q.Tipo_Cliente, q.Desde, q.Hasta);
            var list  = await _repo.GetAllAsync(q.Q, q.Estado, q.Tipo_Cliente, q.Desde, q.Hasta, q.Page, q.PageSize);

            var items = list.Select(ToDto).ToList();
            return (items, total, q.Page, q.PageSize);
        }

        // ----------------------------
        // Obtener por Id
        // ----------------------------
        public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
        {
            var c = await _repo.GetByIdAsync(id);
            return c is null ? null : ToDto(c);
        }

        // ----------------------------
        // Crear
        // ----------------------------
        public async Task<ClienteDto> CrearAsync(ClienteCreateRequest req)
        {
            // Validación de correo duplicado (si viene)
            if (!string.IsNullOrWhiteSpace(req.Correo))
            {
                var existeCorreo = (await _repo.GetAllAsync(q: req.Correo, page: 1, pageSize: 1))
                    .Any(x => x.Correo == req.Correo);
                if (existeCorreo)
                    throw new InvalidOperationException("El correo ya está registrado para otro cliente.");
            }

            var entity = new Cliente
            {
                Nombre         = req.Nombre.Trim(),
                Apellido       = req.Apellido,
                Correo         = req.Correo,
                Telefono       = req.Telefono,
                Direccion      = req.Direccion,
                Tipo_Cliente   = string.IsNullOrWhiteSpace(req.Tipo_Cliente) ? "Normal" : req.Tipo_Cliente!,
                Estado         = string.IsNullOrWhiteSpace(req.Estado) ? "Activo" : req.Estado!,
                Fecha_Registro = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return ToDto(entity);
        }

        // ----------------------------
        // Actualizar
        // ----------------------------
        public async Task<ClienteDto?> ActualizarAsync(int id, ClienteUpdateRequest req)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return null;

            // Validar correo duplicado si lo envían
            if (!string.IsNullOrWhiteSpace(req.Correo))
            {
                var existe = (await _repo.GetAllAsync(q: req.Correo, page: 1, pageSize: 5))
                    .Any(c => c.Id_Cliente != id && c.Correo == req.Correo);
                if (existe)
                    throw new InvalidOperationException("El correo ya está registrado para otro cliente.");
            }

            ApplyUpdate(entity, req);

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            return ToDto(entity);
        }

        // ----------------------------
        // Eliminar
        // ----------------------------
        public async Task<bool> EliminarAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return false;

            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
