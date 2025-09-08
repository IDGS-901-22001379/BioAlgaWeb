using BioAlga.Backend.Dtos.Dashboard;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BioAlga.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ============================
        // Ventas Resumen (día, semana, mes, año)
        // ============================
        [HttpGet("ventas/resumen")]
        [ProducesResponseType(typeof(IEnumerable<VentasResumenDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVentasResumen()
        {
            var data = await _dashboardService.GetVentasResumenAsync();
            return Ok(data);
        }

        // ============================
        // Top Productos (unidades e ingreso)
        // ============================
        [HttpGet("top/productos")]
        [ProducesResponseType(typeof(IEnumerable<TopProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopProductos()
        {
            var data = await _dashboardService.GetTopProductosAsync();
            return Ok(data);
        }

        // ============================
        // Top Clientes
        // ============================
        [HttpGet("top/clientes")]
        [ProducesResponseType(typeof(IEnumerable<TopClienteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopClientes()
        {
            var data = await _dashboardService.GetTopClientesAsync();
            return Ok(data);
        }

        // ============================
        // Ventas por Usuario
        // ============================
        [HttpGet("ventas/usuarios")]
        [ProducesResponseType(typeof(IEnumerable<VentasPorUsuarioDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVentasPorUsuarios()
        {
            var data = await _dashboardService.GetVentasPorUsuariosAsync();
            return Ok(data);
        }

        // ============================
        // Devoluciones por Usuario
        // ============================
        [HttpGet("devoluciones/usuarios")]
        [ProducesResponseType(typeof(IEnumerable<DevolucionesPorUsuarioDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDevolucionesPorUsuarios()
        {
            var data = await _dashboardService.GetDevolucionesPorUsuariosAsync();
            return Ok(data);
        }

        // ============================
        // Compras por Proveedor
        // ============================
        [HttpGet("compras/proveedores")]
        [ProducesResponseType(typeof(IEnumerable<ComprasPorProveedorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComprasPorProveedores()
        {
            var data = await _dashboardService.GetComprasPorProveedoresAsync();
            return Ok(data);
        }
    }
}
