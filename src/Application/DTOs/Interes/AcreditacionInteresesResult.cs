using Domain.ValueObjects;
using System.Collections.Generic;

namespace Fast_Bank.Application.DTOs.Interes
{
    public class AcreditacionInteresesResult
    {
        public int CuentasProcesadas { get; set; }
        public int CuentasOmitidas { get; set; }
        public double MontoTotalAcreditado { get; set; }
        public List<string> Errores { get; set; } = new();
        public List<DetalleAcreditacion> DetallesPorCuenta { get; set; } = new();
    }
}
