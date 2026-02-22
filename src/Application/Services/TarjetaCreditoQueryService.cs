using System;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Fast_Bank.Application.Services;

public class TarjetaCreditoQueryService
{
    private readonly IDdContext _context;

    public TarjetaCreditoQueryService(IDdContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TarjetaCreditoDto?> ObtenerPorNumeroAsync(string numeroTarjeta)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta))
            throw new ArgumentException("Número de tarjeta inválido.", nameof(numeroTarjeta));

        var tarjeta = await _context.TarjetasCredito
            .FirstOrDefaultAsync(t => t.NumeroTarjeta == numeroTarjeta);

        if (tarjeta == null)
            return null;

        return MapToDto(tarjeta);
    }

    private static TarjetaCreditoDto MapToDto(TarjetaCredito tarjeta)
    {
        return new TarjetaCreditoDto
        {
            NumeroTarjeta = tarjeta.NumeroTarjeta,
            LimiteCredito = Math.Round(tarjeta.LimiteCredito, 2, MidpointRounding.ToEven),
            SaldoUtilizado = Math.Round(tarjeta.SaldoUtilizado, 2, MidpointRounding.ToEven),
            CreditoDisponible = Math.Round(tarjeta.CreditoDisponible, 2, MidpointRounding.ToEven),
            FechaEmision = tarjeta.FechaEmision,
            FechaVencimiento = tarjeta.FechaVencimiento,
            TasaInteresMensual = Math.Round(tarjeta.TasaInteresMensual, 4, MidpointRounding.ToEven),
            PagoMinimo = Math.Round(tarjeta.PagoMinimo, 2, MidpointRounding.ToEven),
            EstaVencida = tarjeta.EstaVencida()
        };
    }
}

// DTO
public class TarjetaCreditoDto
{
    public string NumeroTarjeta { get; set; } = string.Empty;
    public double LimiteCredito { get; set; }
    public double SaldoUtilizado { get; set; }
    public double CreditoDisponible { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public double TasaInteresMensual { get; set; }
    public double PagoMinimo { get; set; }
    public bool EstaVencida { get; set; }
}
