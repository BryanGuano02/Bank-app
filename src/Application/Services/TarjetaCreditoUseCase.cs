using System;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using Fast_Bank.Domain.Patterns.StrategyTarjetaCredito;
using DomainTarjetaCreditoService = Domain.Services.TarjetaCreditoService;

namespace Fast_Bank.Application.Services;

public class TarjetaCreditoUseCase
{
    private readonly IDdContext _context;
    private readonly DomainTarjetaCreditoService _domainTarjetaCreditoService;

    public TarjetaCreditoUseCase(IDdContext context, DomainTarjetaCreditoService domainTarjetaCreditoService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainTarjetaCreditoService = domainTarjetaCreditoService ?? throw new ArgumentNullException(nameof(domainTarjetaCreditoService));
    }

    public async Task<string> RealizarCompraAsync(string numeroTarjeta, double monto, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta))
            throw new ArgumentException("Número de tarjeta inválido.", nameof(numeroTarjeta));

        if (monto <= 0)
            throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");

        var tarjeta = await _context.TarjetasCredito.FindAsync(numeroTarjeta);
        if (tarjeta == null)
            throw new InvalidOperationException($"Tarjeta de crédito {numeroTarjeta} no encontrada.");

        var estrategia = new CompraTarjetaStrategy();
        _domainTarjetaCreditoService.EjecutarOperacion(estrategia, tarjeta, monto, descripcion ?? string.Empty);

        await _context.SaveChangesAsync();

        return tarjeta.NumeroTarjeta;
    }

    public async Task<string> RealizarPagoAsync(string numeroTarjeta, double monto)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta))
            throw new ArgumentException("Número de tarjeta inválido.", nameof(numeroTarjeta));

        if (monto <= 0)
            throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");

        var tarjeta = await _context.TarjetasCredito.FindAsync(numeroTarjeta);
        if (tarjeta == null)
            throw new InvalidOperationException($"Tarjeta de crédito {numeroTarjeta} no encontrada.");

        var estrategia = new PagoTarjetaStrategy();
        _domainTarjetaCreditoService.EjecutarOperacion(estrategia, tarjeta, monto, "Pago de tarjeta");

        await _context.SaveChangesAsync();

        return tarjeta.NumeroTarjeta;
    }
}
