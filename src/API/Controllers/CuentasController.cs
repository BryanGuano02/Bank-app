using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly CuentaService _cuentaService;
    private readonly CuentaQueryService _cuentaQueryService;

    public CuentasController(
        CuentaService cuentaService,
        CuentaQueryService cuentaQueryService)
    {
        _cuentaService = cuentaService;
        _cuentaQueryService = cuentaQueryService;
    }

    // DTO para Cuenta de Ahorros
    public class CrearCuentaAhorrosRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        
        /// <summary>
        /// Tasa de inter�s anual en formato decimal.
        /// </summary>
        public double TasaInteres { get; set; } = 2;
    }

    // DTO para Cuenta Corriente
    public class CrearCuentaCorrienteRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public decimal LimiteSobregiro { get; set; } = 500;
    }

    /// <summary>
    /// Crear una nueva cuenta de ahorros
    /// </summary>
    [HttpPost("ahorros")]
    public async Task<IActionResult> CrearCuentaAhorros([FromBody] CrearCuentaAhorrosRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuenta))
            return BadRequest("NumeroCuenta es requerido.");

        try
        {
            var cuenta = await _cuentaService.CrearCuentaAhorrosAsync(
                req.NumeroCuenta,
                req.SaldoInicial,
                req.TasaInteres
            );

            return CreatedAtAction(nameof(Get), new { numero = cuenta.NumeroCuenta }, new
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = "Ahorros",
                SaldoInicial = req.SaldoInicial,
                TasaInteres = req.TasaInteres,
                FechaApertura = cuenta.FechaApertura
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Crear una nueva cuenta corriente
    /// </summary>
    [HttpPost("corriente")]
    public async Task<IActionResult> CrearCuentaCorriente([FromBody] CrearCuentaCorrienteRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuenta))
            return BadRequest("NumeroCuenta es requerido.");

        try
        {
            var cuenta = await _cuentaService.CrearCuentaCorrienteAsync(
                req.NumeroCuenta,
                req.SaldoInicial,
                req.LimiteSobregiro
            );

            return CreatedAtAction(nameof(Get), new { numero = cuenta.NumeroCuenta }, new
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = "Corriente",
                SaldoInicial = req.SaldoInicial,
                LimiteSobregiro = req.LimiteSobregiro,
                FechaApertura = cuenta.FechaApertura
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
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
