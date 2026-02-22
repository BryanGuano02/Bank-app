namespace Domain.ValueObjects
{
    /// <summary>
    /// Resultado inmutable que retorna CuentaAhorros.AplicarInteresMensual().
    /// Vive en el Dominio porque es producido por la entidad.
    /// </summary>
    public sealed class DetalleAcreditacion
    {
        public string NumeroCuenta { get; init; } = string.Empty;
        public double SaldoAnterior { get; init; }
        public double MontoInteres { get; init; }
        public double SaldoNuevo { get; init; }
        public double TasaAplicada { get; init; }
    }
}
