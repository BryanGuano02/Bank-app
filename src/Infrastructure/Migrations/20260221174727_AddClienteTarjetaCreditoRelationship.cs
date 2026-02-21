using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast_Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteTarjetaCreditoRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarjetasCredito_Clientes_ClienteCedula",
                table: "TarjetasCredito");

            migrationBuilder.DropIndex(
                name: "IX_TarjetasCredito_ClienteCedula",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "ClienteCedula",
                table: "TarjetasCredito");

            migrationBuilder.RenameColumn(
                name: "SaldoPendiente",
                table: "TarjetasCredito",
                newName: "IdCliente");

            migrationBuilder.AlterColumn<double>(
                name: "LimiteCredito",
                table: "TarjetasCredito",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<double>(
                name: "CreditoDisponible",
                table: "TarjetasCredito",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEmision",
                table: "TarjetasCredito",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                table: "TarjetasCredito",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "PagoMinimo",
                table: "TarjetasCredito",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SaldoUtilizado",
                table: "TarjetasCredito",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TasaInteresMensual",
                table: "TarjetasCredito",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasCredito_IdCliente",
                table: "TarjetasCredito",
                column: "IdCliente",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TarjetasCredito_Clientes_IdCliente",
                table: "TarjetasCredito",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "Cedula",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarjetasCredito_Clientes_IdCliente",
                table: "TarjetasCredito");

            migrationBuilder.DropIndex(
                name: "IX_TarjetasCredito_IdCliente",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "CreditoDisponible",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "FechaEmision",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "PagoMinimo",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "SaldoUtilizado",
                table: "TarjetasCredito");

            migrationBuilder.DropColumn(
                name: "TasaInteresMensual",
                table: "TarjetasCredito");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "TarjetasCredito",
                newName: "SaldoPendiente");

            migrationBuilder.AlterColumn<decimal>(
                name: "LimiteCredito",
                table: "TarjetasCredito",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<string>(
                name: "ClienteCedula",
                table: "TarjetasCredito",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasCredito_ClienteCedula",
                table: "TarjetasCredito",
                column: "ClienteCedula");

            migrationBuilder.AddForeignKey(
                name: "FK_TarjetasCredito_Clientes_ClienteCedula",
                table: "TarjetasCredito",
                column: "ClienteCedula",
                principalTable: "Clientes",
                principalColumn: "Cedula");
        }
    }
}
