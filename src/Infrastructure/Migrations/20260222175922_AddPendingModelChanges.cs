using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast_Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Cuentas_DestinoNumeroCuenta",
                table: "Movimientos");

            migrationBuilder.AlterColumn<string>(
                name: "DestinoNumeroCuenta",
                table: "Movimientos",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Cuentas_DestinoNumeroCuenta",
                table: "Movimientos",
                column: "DestinoNumeroCuenta",
                principalTable: "Cuentas",
                principalColumn: "NumeroCuenta",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Cuentas_DestinoNumeroCuenta",
                table: "Movimientos");

            migrationBuilder.AlterColumn<string>(
                name: "DestinoNumeroCuenta",
                table: "Movimientos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Cuentas_DestinoNumeroCuenta",
                table: "Movimientos",
                column: "DestinoNumeroCuenta",
                principalTable: "Cuentas",
                principalColumn: "NumeroCuenta");
        }
    }
}
