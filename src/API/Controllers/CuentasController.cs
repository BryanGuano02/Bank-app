using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Fast_Bank.Infrastructure.Persistence;
using Domain.Entities;
using Domain.Patterns.State;

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

    public class CrearCuentaRequest
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public double TasaInteres { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCuentaRequest req)
    {
        if (req == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(req.NumeroCuenta)) return BadRequest("NumeroCuenta es requerido.");

        var existente = await _context.Cuentas.FindAsync(req.NumeroCuenta);
        if (existente != null) return Conflict("La cuenta ya existe.");

        var cuenta = CuentaAhorros.Create(req.NumeroCuenta, req.SaldoInicial, req.TasaInteres, new EstadoCuentaActiva());

        await _context.Cuentas.AddAsync(cuenta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { numero = cuenta.NumeroCuenta }, new { NumeroCuenta = cuenta.NumeroCuenta });
    }

    [HttpGet("{numero}")]
    public async Task<IActionResult> Get(string numero)
    {
        var cuenta = await _context.Cuentas.FindAsync(numero);
        if (cuenta == null) return NotFound();

        return Ok(new { NumeroCuenta = cuenta.NumeroCuenta, Saldo = cuenta.Saldo });
    }
}
