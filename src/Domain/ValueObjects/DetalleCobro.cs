namespace Domain.ValueObjects
{
    public sealed class DetalleCobro
    {
        public string NumeroCuenta { get; init; } = string.Empty;
        public double SaldoAnterior { get; init; }
        public double MontoInteres { get; init; }
        public double SaldoNuevo { get; init; }
        public double TasaAplicada { get; init; }
    }
}
