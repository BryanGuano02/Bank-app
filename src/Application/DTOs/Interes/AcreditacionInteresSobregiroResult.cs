using Domain.ValueObjects;
using System.Collections.Generic;

namespace Fast_Bank.Application.DTOs.Interes
{
    public class AcreditacionInteresSobregiroResult
    {
        public int CuentasProcesadas { get; set; }
        public int CuentasOmitidas { get; set; }
        public double MontoTotalCobrado { get; set; }
        public List<string> Errores { get; set; } = new();
        public List<DetalleCobro> DetallesPorCuenta { get; set; } = new();
    }
}
