using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;
using Domain.Entities;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly ClienteUseCase _clienteService;

    public ClienteController(ClienteUseCase clienteService)
    {
        _clienteService = clienteService;
    }

    public class CrearClienteConCuentaCorrienteRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
    }

    public class CrearClienteConCuentaAhorrosRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public double TasaInteres { get; set; } = 2;
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

    [HttpPost("con-cuenta-corriente")]
    public async Task<IActionResult> CrearConCuentaCorriente([FromBody] CrearClienteConCuentaCorrienteRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.Cedula)) return BadRequest("Cédula es requerida.");

        try
        {
            var cliente = await _clienteService.CrearClienteConCuentaCorrienteAsync(
                req.Cedula,
                req.Nombre,
                req.Apellido,
                req.Direccion,
                req.Correo,
                req.Telefono,
                req.SaldoInicial
            );

            var cuentaCorriente = cliente.Cuenta as CuentaCorriente;
            var location = $"/api/cliente/{cliente.Cedula}";
            return Created(location, new
            {
                Cedula = cliente.Cedula,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Cuenta = new
                {
                    NumeroCuenta = cliente.Cuenta?.NumeroCuenta,
                    TipoCuenta = "Corriente",
                    Saldo = cliente.Cuenta?.Saldo,
                    LimiteSobregiro = cuentaCorriente?.LimiteSobregiro,
                    InteresSobregiro = cuentaCorriente?.InteresSobregiro,
                    FechaApertura = cliente.Cuenta?.FechaApertura
                }
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

    [HttpPost("con-cuenta-ahorros")]
    public async Task<IActionResult> CrearConCuentaAhorros([FromBody] CrearClienteConCuentaAhorrosRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.Cedula)) return BadRequest("Cédula es requerida.");

        try
        {
            var cliente = await _clienteService.CrearClienteConCuentaAhorrosAsync(
                req.Cedula,
                req.Nombre,
                req.Apellido,
                req.Direccion,
                req.Correo,
                req.Telefono,
                req.SaldoInicial,
                req.TasaInteres
            );

            var cuentaAhorros = cliente.Cuenta as CuentaAhorros;
            var location = $"/api/cliente/{cliente.Cedula}";
            return Created(location, new
            {
                Cedula = cliente.Cedula,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Cuenta = new
                {
                    NumeroCuenta = cliente.Cuenta?.NumeroCuenta,
                    TipoCuenta = "Ahorros",
                    Saldo = cliente.Cuenta?.Saldo,
                    TasaInteres = cuentaAhorros?.TasaInteres,
                    FechaApertura = cliente.Cuenta?.FechaApertura
                }
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await _clienteService.GetAllClientesAsync();
        return Ok(clientes);
    }

    [HttpGet("{cedula}")]
    public async Task<IActionResult> GetById(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula)) return BadRequest("Cédula es requerida.");

        var cliente = await _clienteService.GetClienteAsync(cedula);
        if (cliente == null) return NotFound(new { error = "Cliente no encontrado." });

        return Ok(cliente);
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

}
