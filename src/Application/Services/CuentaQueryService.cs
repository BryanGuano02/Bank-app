using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fast_Bank.Application.Services
{
    public class CuentaQueryService
    {
        private readonly IDdContext _context;

        public CuentaQueryService(IDdContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CuentaDto?> ObtenerPorNumeroAsync(string numeroCuenta)
        {
            var cuenta = await _context.Cuentas.FindAsync(numeroCuenta);
            
            if (cuenta == null)
                return null;

            return MapToDto(cuenta);
        }

        public async Task<IEnumerable<CuentaDto>> ObtenerTodasAsync()
        {
            var cuentas = await _context.Cuentas.ToListAsync();
            return cuentas.Select(MapToDto);
        }

        private static CuentaDto MapToDto(Cuenta cuenta)
        {
            if (cuenta is CuentaCorriente cuentaCorriente)
            {
                return new CuentaDto
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = "Corriente",
                    Saldo = cuenta.Saldo,
                    FechaApertura = cuenta.FechaApertura,
                    LimiteSobregiro = cuentaCorriente.LimiteSobregiro,
                    SaldoDisponible = cuenta.Saldo + cuentaCorriente.LimiteSobregiro
                };
            }
            else if (cuenta is CuentaAhorros cuentaAhorros)
            {
                return new CuentaDto
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = "Ahorros",
                    Saldo = cuenta.Saldo,
                    FechaApertura = cuenta.FechaApertura,
                    TasaInteres = cuentaAhorros.TasaInteres
                };
            }
            else
            {
                return new CuentaDto
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = "Desconocido",
                    Saldo = cuenta.Saldo,
                    FechaApertura = cuenta.FechaApertura
                };
            }
        }
    }

    // DTO
    public class CuentaDto
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public DateTime FechaApertura { get; set; }
        
        // Propiedades opcionales según tipo de cuenta
        public double? TasaInteres { get; set; }
        public decimal? LimiteSobregiro { get; set; }
        public decimal? SaldoDisponible { get; set; }
    }
}