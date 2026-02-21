namespace Fast_Bank.Application.DTOs.Cuenta
{
    public class CuentaCorrienteDto : CuentaDtoBase
    {
        public decimal LimiteSobregiro { get; set; }
        public decimal SaldoDisponible { get; set; }
    }
}
