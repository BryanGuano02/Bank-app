using Fast_Bank.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fast_Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InteresesController : ControllerBase
    {
        private readonly InteresesService _interesesService;

        public InteresesController(InteresesService interesesService)
        {
            _interesesService = interesesService;
        }

        [HttpPost("acreditar-todos")]
        public async Task<IActionResult> AcreditarInteresesATodas()
        {
            try
            {
                var resultado = await _interesesService.AcreditarInteresesMensualesAsync();

                return Ok(new
                {
                    Mensaje = "Proceso de acreditación de intereses completado",
                    CuentasProcesadas = resultado.CuentasProcesadas,
                    CuentasOmitidas = resultado.CuentasOmitidas,
                    MontoTotalAcreditado = resultado.MontoTotalAcreditado,
                    Detalles = resultado.DetallesPorCuenta,
                    Errores = resultado.Errores.Count > 0 ? resultado.Errores : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al procesar intereses", detalle = ex.Message });
            }
        }

        [HttpPost("acreditar/{numeroCuenta}")]
        public async Task<IActionResult> AcreditarInteresACuenta(string numeroCuenta)
        {
            try
            {
                var detalle = await _interesesService.AcreditarInteresACuentaAsync(numeroCuenta);

                return Ok(new
                {
                    Mensaje = "Interés acreditado exitosamente",
                    NumeroCuenta = detalle.NumeroCuenta,
                    SaldoAnterior = detalle.SaldoAnterior,
                    InteresAcreditado = detalle.MontoInteres,
                    SaldoNuevo = detalle.SaldoNuevo,
                    TasaAplicada = $"{detalle.TasaAplicada:P2}"
                });
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

        [HttpGet("simular/{numeroCuenta}")]
        public async Task<IActionResult> SimularInteres(string numeroCuenta)
        {
            try
            {
                var simulacion = await _interesesService.SimularInteresAsync(numeroCuenta);

                return Ok(new
                {
                    NumeroCuenta = simulacion.NumeroCuenta,
                    SaldoActual = simulacion.SaldoActual,
                    TasaInteresAnual = simulacion.TasaInteresAnual,
                    TasaInteresMensual = simulacion.TasaInteresMensual,
                    InteresCalculado = simulacion.InteresCalculado,
                    SaldoProyectado = simulacion.SaldoProyectado
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}