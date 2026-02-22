using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast_Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteCedulaToCuenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TasaInteres",
                table: "Cuentas");

            migrationBuilder.AlterColumn<double>(
                name: "Monto",
                table: "Movimientos",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "Saldo",
                table: "Cuentas",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "LimiteSobregiro",
                table: "Cuentas",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "InteresSobregiro",
                table: "Cuentas",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Monto",
                table: "Movimientos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "Saldo",
                table: "Cuentas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "LimiteSobregiro",
                table: "Cuentas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "InteresSobregiro",
                table: "Cuentas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TasaInteres",
                table: "Cuentas",
                type: "REAL",
                nullable: true);
        }
    }
}
