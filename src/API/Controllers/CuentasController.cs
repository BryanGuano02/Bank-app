using Domain.Entities;
using Domain.Patterns.State;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Fast_Bank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly DdContext _context;

    public CuentasController(DdContext context)
    {
        _context = context;
    }

    // DTO para Cuenta de Ahorros
    public class CrearCuentaAhorrosRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public double TasaInteres { get; set; } = 0.02; // 2% por defecto
    }

    // DTO para Cuenta Corriente
    public class CrearCuentaCorrienteRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public decimal LimiteSobregiro { get; set; } = 500; // $500 por defecto
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

        var existente = await _context.Cuentas.FindAsync(req.NumeroCuenta);
        if (existente != null)
            return Conflict("La cuenta ya existe.");

        var cuenta = CuentaAhorros.Create(
            req.NumeroCuenta,
            req.SaldoInicial,
            req.TasaInteres,
            new EstadoCuentaActiva()
        );

        await _context.Cuentas.AddAsync(cuenta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { numero = cuenta.NumeroCuenta }, new
        {
            NumeroCuenta = cuenta.NumeroCuenta,
            TipoCuenta = "Ahorros",
            SaldoInicial = req.SaldoInicial,
            TasaInteres = req.TasaInteres,
            FechaApertura = cuenta.FechaApertura
        });
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

        var existente = await _context.Cuentas.FindAsync(req.NumeroCuenta);
        if (existente != null)
            return Conflict("La cuenta ya existe.");

        var cuenta = CuentaCorriente.Create(
            req.NumeroCuenta,
            req.SaldoInicial,
            req.LimiteSobregiro,
            new EstadoCuentaActiva()
        );

        await _context.Cuentas.AddAsync(cuenta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { numero = cuenta.NumeroCuenta }, new
        {
            NumeroCuenta = cuenta.NumeroCuenta,
            TipoCuenta = "Corriente",
            SaldoInicial = req.SaldoInicial,
            LimiteSobregiro = req.LimiteSobregiro,
            FechaApertura = cuenta.FechaApertura
        });
    }

    /// <summary>
    /// Consultar una cuenta por número
    /// </summary>
    [HttpGet("{numero}")]
    public async Task<IActionResult> Get(string numero)
    {
        var cuenta = await _context.Cuentas.FindAsync(numero);
        if (cuenta == null) return NotFound();

        // Retornar información según el tipo de cuenta
        object response;

        if (cuenta is CuentaCorriente cuentaCorriente)
        {
            response = new
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                Saldo = cuenta.Saldo,
                TipoCuenta = "Corriente",
                LimiteSobregiro = cuentaCorriente.LimiteSobregiro,
                SaldoDisponible = cuenta.Saldo + cuentaCorriente.LimiteSobregiro,
                FechaApertura = cuenta.FechaApertura
            };
        }
        else if (cuenta is CuentaAhorros cuentaAhorros)
        {
            response = new
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                Saldo = cuenta.Saldo,
                TipoCuenta = "Ahorros",
                TasaInteres = cuentaAhorros.TasaInteres,
                FechaApertura = cuenta.FechaApertura
            };
        }
        else
        {
            response = new
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                Saldo = cuenta.Saldo,
                FechaApertura = cuenta.FechaApertura
            };
        }

        return Ok(response);
    }

    /// <summary>
    /// Listar todas las cuentas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cuentas = await _context.Cuentas.ToListAsync();

        var response = cuentas.Select<Cuenta, object>(cuenta =>
        {
            if (cuenta is CuentaCorriente cc)
            {
                return new
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = "Corriente",
                    Saldo = cuenta.Saldo,
                    LimiteSobregiro = cc.LimiteSobregiro,
                    FechaApertura = cuenta.FechaApertura
                };
            }
            else if (cuenta is CuentaAhorros ca)
            {
                return new
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = "Ahorros",
                    Saldo = cuenta.Saldo,
                    TasaInteres = ca.TasaInteres,
                    FechaApertura = cuenta.FechaApertura
                };
            }
            else
            {
                return new
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = "Desconocido",
                    Saldo = cuenta.Saldo,
                    FechaApertura = cuenta.FechaApertura
                };
            }
        });

        return Ok(response);
    }
}
