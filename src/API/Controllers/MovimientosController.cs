using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly MovimientoService _movimientoService;
    private readonly MovimientoQueryService _movimientoQueryService;

    public MovimientosController(
        MovimientoService movimientoService,
        MovimientoQueryService movimientoQueryService)
    {
        _movimientoService = movimientoService;
        _movimientoQueryService = movimientoQueryService;
    }

    public class DepositoRequest
    {
        public string NumeroCuentaDestino { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class RetiroRequest
    {
        public string NumeroCuentaOrigen { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class TransferenciaRequest
    {
        public string NumeroCuentaOrigen { get; set; } = string.Empty;
        public string NumeroCuentaDestino { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    /// <summary>
    /// Realizar un depósito
    /// </summary>
    [HttpPost("depositar")]
    public async Task<IActionResult> Depositar([FromBody] DepositoRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaDestino)) 
            return BadRequest("NumeroCuentaDestino es requerido.");
        if (req.Monto <= 0) 
            return BadRequest("Monto debe ser mayor que cero.");

        var id = await _movimientoService.DepositarAsync(
            req.NumeroCuentaDestino, 
            req.Monto, 
            req.Descripcion ?? string.Empty);

        return CreatedAtAction(nameof(GetById), new { id }, new { IdMovimiento = id });
    }

    /// <summary>
    /// Realizar un retiro
    /// </summary>
    [HttpPost("retirar")]
    public async Task<IActionResult> Retirar([FromBody] RetiroRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaOrigen)) 
            return BadRequest("NumeroCuentaOrigen es requerido.");
        if (req.Monto <= 0) 
            return BadRequest("Monto debe ser mayor que cero.");

        try
        {
            var id = await _movimientoService.RetirarAsync(
                req.NumeroCuentaOrigen, 
                req.Monto, 
                req.Descripcion ?? string.Empty);
            return CreatedAtAction(nameof(GetById), new { id }, new { IdMovimiento = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Realizar una transferencia
    /// </summary>
    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaOrigen)) 
            return BadRequest("NumeroCuentaOrigen es requerido.");
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaDestino)) 
            return BadRequest("NumeroCuentaDestino es requerido.");
        if (req.Monto <= 0) 
            return BadRequest("Monto debe ser mayor que cero.");

        try
        {
            var id = await _movimientoService.TransferirAsync(
                req.NumeroCuentaOrigen, 
                req.NumeroCuentaDestino, 
                req.Monto, 
                req.Descripcion ?? string.Empty);
            return CreatedAtAction(nameof(GetById), new { id }, 
                new { IdMovimiento = id, Mensaje = "Transferencia completada exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

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
