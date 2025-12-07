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
                name: "Cuentas",
                columns: table => new
                {
                    NumeroCuenta = table.Column<string>(type: "TEXT", nullable: false),
                    Saldo = table.Column<decimal>(type: "TEXT", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoCuenta = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    TasaInteres = table.Column<double>(type: "REAL", nullable: true),
                    LimiteSobregiro = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.NumeroCuenta);
                });

            migrationBuilder.CreateTable(
                name: "EntidadesFinancieras",
                columns: table => new
                {
                    IdEntidad = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Ciudad = table.Column<string>(type: "TEXT", nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntidadesFinancieras", x => x.IdEntidad);
                });

            migrationBuilder.CreateTable(
                name: "TarjetasCredito",
                columns: table => new
                {
                    NumeroTarjeta = table.Column<string>(type: "TEXT", nullable: false),
                    LimiteCredito = table.Column<decimal>(type: "TEXT", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClienteCedula = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarjetasCredito", x => x.NumeroTarjeta);
                    table.ForeignKey(
                        name: "FK_TarjetasCredito_Clientes_ClienteCedula",
                        column: x => x.ClienteCedula,
                        principalTable: "Clientes",
                        principalColumn: "Cedula");
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    IdMovimiento = table.Column<string>(type: "TEXT", nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", nullable: false),
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
                name: "IX_Movimientos_DestinoNumeroCuenta",
                table: "Movimientos",
                column: "DestinoNumeroCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_OrigenNumeroCuenta",
                table: "Movimientos",
                column: "OrigenNumeroCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasCredito_ClienteCedula",
                table: "TarjetasCredito",
                column: "ClienteCedula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntidadesFinancieras");

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
