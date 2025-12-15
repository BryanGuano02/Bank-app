using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly MovimientoQueryService _movimientoQueryService;

    public MovimientosController(MovimientoQueryService movimientoQueryService)
    {
        _movimientoQueryService = movimientoQueryService;
    }

    // Las operaciones de creación de movimientos se movieron a `CuentasController`.

    /// <summary>
    /// Consultar un movimiento por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var movimiento = await _movimientoQueryService.ObtenerPorIdAsync(id);

        if (movimiento == null)
            return NotFound(new { error = "Movimiento no encontrado." });

        return Ok(movimiento);
    }

    /// <summary>
    /// Listar todos los movimientos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movimientos = await _movimientoQueryService.ObtenerTodosAsync();
        return Ok(movimientos);
    }

    /// <summary>
    /// Obtener movimientos de una cuenta específica
    /// </summary>
    [HttpGet("cuenta/{numeroCuenta}")]
    public async Task<IActionResult> GetByNumeroCuenta(string numeroCuenta)
    {
        var movimientos = await _movimientoQueryService.ObtenerPorCuentaAsync(numeroCuenta);

        if (!movimientos.Any())
            return Ok(new List<MovimientoDto>()); // Retorna lista vacía en lugar de 404

        return Ok(movimientos);
    }
}
