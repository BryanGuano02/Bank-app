using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;
using Domain.Entities;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly ClienteService _clienteService;
    private readonly CuentaService _cuentaService;

    public ClienteController(ClienteService clienteService, CuentaService cuentaService)
    {
        _clienteService = clienteService;
        _cuentaService = cuentaService;
    }

    public class CrearClienteRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    public class ActualizarClienteRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    // DTOs para creación de cuentas a través del cliente
    public class CrearCuentaAhorrosRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public double TasaInteres { get; set; } = 2;
    }

    public class CrearCuentaCorrienteRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public decimal LimiteSobregiro { get; set; } = 500;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearClienteRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.Cedula)) return BadRequest("Cédula es requerida.");

        try
        {
            var cliente = await _clienteService.CrearClienteAsync(
                req.Cedula,
                req.Nombre,
                req.Apellido,
                req.Direccion,
                req.Correo,
                req.Telefono
            );

            var location = $"/api/cliente/{cliente.Cedula}";
            return Created(location, new { Cedula = cliente.Cedula });
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

    [HttpPut("{cedula}")]
    public async Task<IActionResult> Actualizar(string cedula, [FromBody] ActualizarClienteRequest req)
    {
        if (string.IsNullOrWhiteSpace(cedula)) return BadRequest("Cédula es requerida.");
        if (req == null) return BadRequest();

        try
        {
            var cliente = await _clienteService.ActualizarClienteAsync(
                cedula,
                req.Nombre,
                req.Apellido,
                req.Direccion,
                req.Correo,
                req.Telefono
            );

            return Ok(new { Cedula = cliente.Cedula });
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

    [HttpPost("{cedula}/cuenta/ahorros")]
    public async Task<IActionResult> CrearCuentaAhorros(string cedula, [FromBody] CrearCuentaAhorrosRequest req)
    {
        if (string.IsNullOrWhiteSpace(cedula)) return BadRequest("Cédula es requerida.");
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuenta)) return BadRequest("NumeroCuenta es requerido.");

        try
        {
            var cuenta = await _cuentaService.CrearCuentaAhorrosAsync(
                cedula,
                req.NumeroCuenta,
                req.SaldoInicial,
                req.TasaInteres
            );

            return Created($"/api/cuentas/{cuenta.NumeroCuenta}", new
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

    [HttpPost("{cedula}/cuenta/corriente")]
    public async Task<IActionResult> CrearCuentaCorriente(string cedula, [FromBody] CrearCuentaCorrienteRequest req)
    {
        if (string.IsNullOrWhiteSpace(cedula)) return BadRequest("Cédula es requerida.");
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuenta)) return BadRequest("NumeroCuenta es requerido.");

        try
        {
            var cuenta = await _cuentaService.CrearCuentaCorrienteAsync(
                cedula,
                req.NumeroCuenta,
                req.SaldoInicial,
                req.LimiteSobregiro
            );

            return Created($"/api/cuentas/{cuenta.NumeroCuenta}", new
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
}
