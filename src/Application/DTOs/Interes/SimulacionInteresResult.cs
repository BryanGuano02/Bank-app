namespace Fast_Bank.Application.DTOs.Interes
{
    public class SimulacionInteresResult
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public double SaldoActual { get; set; }
        public double TasaInteresAnual { get; set; }
        public double TasaInteresMensual { get; set; }
        public double InteresCalculado { get; set; }
        public double SaldoProyectado { get; set; }
    }
}
