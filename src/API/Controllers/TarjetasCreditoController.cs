using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarjetasCreditoController : ControllerBase
{
    private readonly TarjetaCreditoQueryService _queryService;
    private readonly TarjetaCreditoUseCase _useCase;

    public TarjetasCreditoController(
        TarjetaCreditoQueryService queryService,
        TarjetaCreditoUseCase useCase)
    {
        _queryService = queryService;
        _useCase = useCase;
    }

    // DTOs para las operaciones de tarjeta
    public class CompraRequest
    {
        public double Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class PagoRequest
    {
        public double Monto { get; set; }
    }

    [HttpPost("cliente/{idCliente}")]
    public async Task<IActionResult> CrearTarjeta(string idCliente)
    {
        if (string.IsNullOrWhiteSpace(idCliente))
            return BadRequest("El ID del cliente es requerido.");

        try
        {
            var numeroTarjeta = await _useCase.CrearTarjetaAsync(idCliente);
            var tarjeta = await _queryService.ObtenerPorNumeroAsync(numeroTarjeta);

            return CreatedAtAction(
                nameof(GetByNumero),
                new { numeroTarjeta },
                new { Mensaje = "Tarjeta de crédito creada y asignada exitosamente.", Tarjeta = tarjeta });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (System.ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener información de una tarjeta de crédito por su número
    /// </summary>
    /// <param name="numeroTarjeta">Número de la tarjeta de crédito</param>
    [HttpGet("{numeroTarjeta}")]
    public async Task<IActionResult> GetByNumero(string numeroTarjeta)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta))
            return BadRequest("Número de tarjeta es requerido.");

        try
        {
            var tarjeta = await _queryService.ObtenerPorNumeroAsync(numeroTarjeta);

            if (tarjeta == null)
                return NotFound(new { error = $"Tarjeta de crédito {numeroTarjeta} no encontrada." });

            return Ok(tarjeta);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Realizar una compra con tarjeta de crédito
    /// </summary>
    /// <param name="numeroTarjeta">Número de la tarjeta de crédito</param>
    /// <param name="req">Datos de la compra (monto y descripción)</param>
    [HttpPost("{numeroTarjeta}/comprar")]
    public async Task<IActionResult> RealizarCompra(string numeroTarjeta, [FromBody] CompraRequest req)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta))
            return BadRequest("Número de tarjeta es requerido.");

        if (req == null)
            return BadRequest("Datos de compra son requeridos.");

        if (req.Monto <= 0)
            return BadRequest("El monto debe ser mayor que cero.");

        try
        {
            await _useCase.RealizarCompraAsync(numeroTarjeta, req.Monto, req.Descripcion);

            return Ok(new
            {
                NumeroTarjeta = numeroTarjeta,
                Monto = req.Monto,
                Descripcion = req.Descripcion,
                Mensaje = "Compra realizada exitosamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (System.ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Realizar un pago a la tarjeta de crédito
    /// </summary>
    /// <param name="numeroTarjeta">Número de la tarjeta de crédito</param>
    /// <param name="req">Datos del pago (monto)</param>
    [HttpPost("{numeroTarjeta}/pagar")]
    public async Task<IActionResult> RealizarPago(string numeroTarjeta, [FromBody] PagoRequest req)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta))
            return BadRequest("Número de tarjeta es requerido.");

        if (req == null)
            return BadRequest("Datos de pago son requeridos.");

        if (req.Monto <= 0)
            return BadRequest("El monto debe ser mayor que cero.");

        try
        {
            await _useCase.RealizarPagoAsync(numeroTarjeta, req.Monto);

            return Ok(new
            {
                NumeroTarjeta = numeroTarjeta,
                Monto = req.Monto,
                Mensaje = "Pago realizado exitosamente."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (System.ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
