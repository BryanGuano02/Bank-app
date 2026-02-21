using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;
using Domain.Entities;
using Domain.Enums;
using System.Text.Json.Serialization;

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

    // ===== NUEVOS DTOs UNIFICADOS =====

    public class CrearClienteConCuentaRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public TipoCuenta TipoCuenta { get; set; }
    }

    public class CrearClienteConCuentaYTarjetaRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public TipoCuenta TipoCuenta { get; set; }
        public string NumeroTarjeta { get; set; } = string.Empty;
        public double LimiteCredito { get; set; }
        public double TasaInteresMensual { get; set; } = 2.5;
    }

    // DTOs antiguos para compatibilidad (Obsoletos)
    [Obsolete("Usar CrearClienteConCuentaRequest con enum TipoCuenta")]
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

    [Obsolete("Usar CrearClienteConCuentaRequest con enum TipoCuenta")]
    public class CrearClienteConCuentaAhorrosRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
    }

    public class ActualizarClienteRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    // ===== NUEVOS ENDPOINTS UNIFICADOS =====

    [HttpPost("con-cuenta")]
    public async Task<IActionResult> CrearClienteConCuenta([FromBody] CrearClienteConCuentaRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.Cedula)) return BadRequest("Cédula es requerida.");

        try
        {
            var cliente = await _clienteService.CrearClienteConCuentaAsync(
                req.Cedula,
                req.Nombre,
                req.Apellido,
                req.Direccion,
                req.Correo,
                req.Telefono,
                req.SaldoInicial,
                req.TipoCuenta
            );

            var location = $"/api/cliente/{cliente.Cedula}";
            return Created(location, new
            {
                Mensaje = $"Cliente y cuenta {req.TipoCuenta} creados exitosamente",
                Cliente = MapToDto(cliente)
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

    [HttpPost("con-cuenta-y-tarjeta")]
    public async Task<IActionResult> CrearClienteConCuentaYTarjeta([FromBody] CrearClienteConCuentaYTarjetaRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.Cedula)) return BadRequest("Cédula es requerida.");
        if (string.IsNullOrWhiteSpace(req.NumeroTarjeta)) return BadRequest("Número de tarjeta es requerido.");

        try
        {
            var cliente = await _clienteService.CrearClienteConCuentaYTarjetaAsync(
                req.Cedula,
                req.Nombre,
                req.Apellido,
                req.Direccion,
                req.Correo,
                req.Telefono,
                req.SaldoInicial,
                req.TipoCuenta,
                req.NumeroTarjeta,
                req.LimiteCredito,
                req.TasaInteresMensual
            );

            var location = $"/api/cliente/{cliente.Cedula}";

            // Recargar cliente con relaciones
            var clienteCompleto = await _clienteService.GetClienteAsync(cliente.Cedula);

            return Created(location, new
            {
                Mensaje = $"Cliente, cuenta {req.TipoCuenta} y tarjeta de crédito creados exitosamente",
                Cliente = MapToDtoConTarjeta(clienteCompleto!)
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

    // ===== ENDPOINTS ANTIGUOS (Deprecated pero funcionales) =====

    [HttpPost("con-cuenta-corriente")]
    [Obsolete("Usar POST /api/cliente/con-cuenta con TipoCuenta=Corriente")]
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
    [Obsolete("Usar POST /api/cliente/con-cuenta con TipoCuenta=Ahorros")]
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
                req.SaldoInicial
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

    // ===== ENDPOINTS CRUD =====

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await _clienteService.GetAllClientesAsync();
        var dtos = clientes.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("{cedula}")]
    public async Task<IActionResult> GetById(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula)) return BadRequest("Cédula es requerida.");

        var cliente = await _clienteService.GetClienteAsync(cedula);
        if (cliente == null) return NotFound(new { error = "Cliente no encontrado." });

        return Ok(MapToDto(cliente));
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

    // ===== MAPPERS =====

    private ClienteDto MapToDto(Cliente cliente)
    {
        CuentaDto? cuentaDto = null;
        if (cliente.Cuenta != null)
        {
            cuentaDto = new CuentaDto
            {
                NumeroCuenta = cliente.Cuenta.NumeroCuenta,
                Saldo = cliente.Cuenta.Saldo,
                FechaApertura = cliente.Cuenta.FechaApertura
            };

            if (cliente.Cuenta is CuentaCorriente cc)
            {
                cuentaDto.TipoCuenta = "Corriente";
                cuentaDto.LimiteSobregiro = cc.LimiteSobregiro;
                cuentaDto.InteresSobregiro = cc.InteresSobregiro;
            }
            else if (cliente.Cuenta is CuentaAhorros ca)
            {
                cuentaDto.TipoCuenta = "Ahorros";
                cuentaDto.TasaInteres = ca.TasaInteres;
            }
        }

        return new ClienteDto
        {
            Cedula = cliente.Cedula,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Direccion = cliente.Direccion,
            Correo = cliente.Correo,
            Telefono = cliente.Telefono,
            Cuenta = cuentaDto
        };
    }

    private ClienteDtoConTarjeta MapToDtoConTarjeta(Cliente cliente)
    {
        var clienteBase = MapToDto(cliente);

        TarjetaDto? tarjetaDto = null;
        if (cliente.TarjetaCredito != null)
        {
            tarjetaDto = new TarjetaDto
            {
                NumeroTarjeta = cliente.TarjetaCredito.NumeroTarjeta,
                LimiteCredito = cliente.TarjetaCredito.LimiteCredito,
                SaldoUtilizado = cliente.TarjetaCredito.SaldoUtilizado,
                CreditoDisponible = cliente.TarjetaCredito.CreditoDisponible,
                TasaInteresMensual = cliente.TarjetaCredito.TasaInteresMensual,
                FechaEmision = cliente.TarjetaCredito.FechaEmision,
                FechaVencimiento = cliente.TarjetaCredito.FechaVencimiento
            };
        }

        return new ClienteDtoConTarjeta
        {
            Cedula = clienteBase.Cedula,
            Nombre = clienteBase.Nombre,
            Apellido = clienteBase.Apellido,
            Direccion = clienteBase.Direccion,
            Correo = clienteBase.Correo,
            Telefono = clienteBase.Telefono,
            Cuenta = clienteBase.Cuenta,
            TarjetaCredito = tarjetaDto
        };
    }

    // ===== DTOs DE RESPUESTA =====

    public class ClienteDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public CuentaDto? Cuenta { get; set; }
    }

    public class ClienteDtoConTarjeta : ClienteDto
    {
        public TarjetaDto? TarjetaCredito { get; set; }
    }

    public class CuentaDto
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public DateTime FechaApertura { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? LimiteSobregiro { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? InteresSobregiro { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TasaInteres { get; set; }
    }

    public class TarjetaDto
    {
        public string NumeroTarjeta { get; set; } = string.Empty;
        public double LimiteCredito { get; set; }
        public double SaldoUtilizado { get; set; }
        public double CreditoDisponible { get; set; }
        public double TasaInteresMensual { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
