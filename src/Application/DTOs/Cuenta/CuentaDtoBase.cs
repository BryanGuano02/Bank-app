using System;

namespace Fast_Bank.Application.DTOs.Cuenta
{
    public class CuentaDtoBase
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public double Saldo { get; set; }
        public DateTime FechaApertura { get; set; }
    }
}
