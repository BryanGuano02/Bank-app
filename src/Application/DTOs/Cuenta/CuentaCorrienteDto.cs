namespace Fast_Bank.Application.DTOs.Cuenta
{
    public class CuentaCorrienteDto : CuentaDtoBase
    {
        public double LimiteSobregiro { get; set; }
        public double SaldoDisponible { get; set; }
    }
}
