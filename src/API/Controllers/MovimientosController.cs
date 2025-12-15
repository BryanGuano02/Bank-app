using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly MovimientoService _movimientoService;

    public MovimientosController(MovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
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

    [HttpPost("depositar")]
    public async Task<IActionResult> Depositar([FromBody] DepositoRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaDestino)) return BadRequest("NumeroCuentaDestino es requerido.");
        if (req.Monto <= 0) return BadRequest("Monto debe ser mayor que cero.");

        var id = await _movimientoService.DepositarAsync(req.NumeroCuentaDestino, req.Monto, req.Descripcion ?? string.Empty);

        return CreatedAtAction(nameof(GetById), new { id }, new { IdMovimiento = id });
    }

    [HttpPost("retirar")]
    public async Task<IActionResult> Retirar([FromBody] RetiroRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaOrigen)) return BadRequest("NumeroCuentaOrigen es requerido.");
        if (req.Monto <= 0) return BadRequest("Monto debe ser mayor que cero.");

        try
        {
            var id = await _movimientoService.RetirarAsync(req.NumeroCuentaOrigen, req.Monto, req.Descripcion ?? string.Empty);

            return CreatedAtAction(nameof(GetById), new { id }, new { IdMovimiento = id });
        }
        catch (InvalidOperationException ex)
        {
            // Captura errores de negocio (saldo insuficiente, sobregiro excedido, cuenta bloqueada, etc.)
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaOrigen)) return BadRequest("NumeroCuentaOrigen es requerido.");
        if (string.IsNullOrWhiteSpace(req.NumeroCuentaDestino)) return BadRequest("NumeroCuentaDestino es requerido.");
        if (req.Monto <= 0) return BadRequest("Monto debe ser mayor que cero.");

        try
        {
            var id = await _movimientoService.TransferirAsync(req.NumeroCuentaOrigen, req.NumeroCuentaDestino, req.Monto, req.Descripcion ?? string.Empty);

            return CreatedAtAction(nameof(GetById), new { id }, new { IdMovimiento = id, Mensaje = "Transferencia completada exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            // Captura errores de negocio (saldo insuficiente, sobregiro excedido, cuenta bloqueada, cuentas inexistentes, etc.)
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Minimal GET placeholder so CreatedAtAction has a target — implementation can be expanded.
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        return Ok(new { Id = id });
    }
}
