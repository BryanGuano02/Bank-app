using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast_Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Cedula = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Correo = table.Column<string>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Cedula);
                });

            migrationBuilder.CreateTable(
                name: "ControlEjecuciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Proceso = table.Column<string>(type: "TEXT", nullable: false),
                    UltimaEjecucion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlEjecuciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    NumeroCuenta = table.Column<string>(type: "TEXT", nullable: false),
                    Saldo = table.Column<double>(type: "TEXT", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClienteCedula = table.Column<string>(type: "TEXT", nullable: true),
                    TipoCuenta = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    TasaInteres = table.Column<double>(type: "REAL", nullable: true),
                    LimiteSobregiro = table.Column<double>(type: "TEXT", nullable: true),
                    InteresSobregiro = table.Column<double>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.NumeroCuenta);
                    table.ForeignKey(
                        name: "FK_Cuentas_Clientes_ClienteCedula",
                        column: x => x.ClienteCedula,
                        principalTable: "Clientes",
                        principalColumn: "Cedula");
                });

            migrationBuilder.CreateTable(
                name: "TarjetasCredito",
                columns: table => new
                {
                    NumeroTarjeta = table.Column<string>(type: "TEXT", nullable: false),
                    LimiteCredito = table.Column<double>(type: "REAL", nullable: false),
                    SaldoUtilizado = table.Column<double>(type: "REAL", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TasaInteresMensual = table.Column<double>(type: "REAL", nullable: false),
                    CreditoDisponible = table.Column<double>(type: "REAL", nullable: false),
                    PagoMinimo = table.Column<double>(type: "REAL", nullable: false),
                    IdCliente = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarjetasCredito", x => x.NumeroTarjeta);
                    table.ForeignKey(
                        name: "FK_TarjetasCredito_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    IdMovimiento = table.Column<string>(type: "TEXT", nullable: false),
                    Monto = table.Column<double>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    OrigenNumeroCuenta = table.Column<string>(type: "TEXT", nullable: true),
                    DestinoNumeroCuenta = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.IdMovimiento);
                    table.ForeignKey(
                        name: "FK_Movimientos_Cuentas_DestinoNumeroCuenta",
                        column: x => x.DestinoNumeroCuenta,
                        principalTable: "Cuentas",
                        principalColumn: "NumeroCuenta");
                    table.ForeignKey(
                        name: "FK_Movimientos_Cuentas_OrigenNumeroCuenta",
                        column: x => x.OrigenNumeroCuenta,
                        principalTable: "Cuentas",
                        principalColumn: "NumeroCuenta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_ClienteCedula",
                table: "Cuentas",
                column: "ClienteCedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_DestinoNumeroCuenta",
                table: "Movimientos",
                column: "DestinoNumeroCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_OrigenNumeroCuenta",
                table: "Movimientos",
                column: "OrigenNumeroCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasCredito_IdCliente",
                table: "TarjetasCredito",
                column: "IdCliente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlEjecuciones");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "TarjetasCredito");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
