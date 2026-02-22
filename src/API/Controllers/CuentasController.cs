using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly CuentaQueryService _cuentaQueryService;
    private readonly MovimientoUseCase _movimientoService;
    private readonly CuentaUseCase _cuentaUseCase;

    public CuentasController(
        CuentaQueryService cuentaQueryService,
        MovimientoUseCase movimientoService,
        CuentaUseCase cuentaUseCase)
    {
        _cuentaQueryService = cuentaQueryService;
        _movimientoService = movimientoService;
        _cuentaUseCase = cuentaUseCase;
    }

    // DTOs para operaciones de movimientos (delegadas a MovimientoService)
    public class DepositoRequest
    {
        public string NumeroCuentaDestino { get; set; } = string.Empty;
        public double Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class RetiroRequest
    {
        public string NumeroCuentaOrigen { get; set; } = string.Empty;
        public double Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    public class TransferenciaRequest
    {
        public string NumeroCuentaOrigen { get; set; } = string.Empty;
        public string NumeroCuentaDestino { get; set; } = string.Empty;
        public double Monto { get; set; }
        public string? Descripcion { get; set; }
    }

    /// <summary>
    /// Realizar un depósito (mueve desde MovimientosController)
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

        return CreatedAtAction(nameof(MovimientosController.GetById), "Movimientos", new { id }, new { IdMovimiento = id });
    }

    /// <summary>
    /// Realizar un retiro (mueve desde MovimientosController)
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
            return CreatedAtAction(nameof(MovimientosController.GetById), "Movimientos", new { id }, new { IdMovimiento = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Realizar una transferencia (mueve desde MovimientosController)
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
            return CreatedAtAction(nameof(MovimientosController.GetById), "Movimientos", new { id },
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

    // La creación de cuentas ahora se realiza vía `ClienteController`.

    [HttpPost("ahorros/acreditar-intereses")]
    public async Task<IActionResult> AcreditarInteresesAhorros()
    {
        try
        {
            var resultado = await _cuentaUseCase.AcreditarInteresesMensualesAsync();
            return Ok(new
            {
                Mensaje = "Proceso de acreditación de intereses completado",
                resultado.CuentasProcesadas,
                resultado.CuentasOmitidas,
                resultado.MontoTotalAcreditado,
                Detalles = resultado.DetallesPorCuenta,
                Errores = resultado.Errores.Count > 0 ? resultado.Errores : null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al procesar intereses", detalle = ex.Message });
        }
    }

    [HttpPost("corrientes/cobrar-sobregiro")]
    public async Task<IActionResult> CobrarInteresSobregiro()
    {
        try
        {
            var resultado = await _cuentaUseCase.AcreditarInteresSobregiroATodas();
            return Ok(new
            {
                Mensaje = "Proceso de cobro de intereses de sobregiro completado",
                resultado.CuentasProcesadas,
                resultado.CuentasOmitidas,
                resultado.MontoTotalCobrado,
                Detalles = resultado.DetallesPorCuenta,
                Errores = resultado.Errores.Count > 0 ? resultado.Errores : null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al procesar intereses de sobregiro", detalle = ex.Message });
        }
    }


    /// <summary>
    /// Consultar una cuenta por n�mero
    /// </summary>
    [HttpGet("{numero}")]
    public async Task<IActionResult> Get(string numero)
    {
        var cuenta = await _cuentaQueryService.ObtenerPorNumeroAsync(numero);

        if (cuenta == null)
            return NotFound(new { error = "Cuenta no encontrada." });

        return Ok(cuenta);
    }

    /// <summary>
    /// Listar todas las cuentas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cuentas = await _cuentaQueryService.ObtenerTodasAsync();
        return Ok(cuentas);
    }
}
