using Fast_Bank.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fast_Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InteresesController : ControllerBase
    {
        private readonly InteresesUseCase _interesesService;

        public InteresesController(InteresesUseCase interesesService)
        {
            _interesesService = interesesService;
        }

        // Endpoints para cuentas de ahorros (acreditar intereses)
        [HttpPost("ahorros/acreditar-todos")]
        public async Task<IActionResult> AcreditarInteresesATodas()
        {
            try
            {
                var resultado = await _interesesService.AcreditarInteresesMensualesAsync();

                return Ok(new
                {
                    Mensaje = "Proceso de acreditaci�n de intereses completado",
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

        // Endpoints para cuentas corrientes (cobrar intereses de sobregiro)
        [HttpPost("sobregiro/cobrar-todos")]
        public async Task<IActionResult> CobrarInteresSobregiroATodas()
        {
            try
            {
                var resultado = await _interesesService.AcreditarInteresSobregiroATodas();

                return Ok(new
                {
                    Mensaje = "Proceso de cobro de intereses de sobregiro completado",
                    CuentasProcesadas = resultado.CuentasProcesadas,
                    CuentasOmitidas = resultado.CuentasOmitidas,
                    MontoTotalCobrado = resultado.MontoTotalCobrado,
                    Detalles = resultado.DetallesPorCuenta,
                    Errores = resultado.Errores.Count > 0 ? resultado.Errores : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al procesar intereses de sobregiro", detalle = ex.Message });
            }
        }
    }
}
