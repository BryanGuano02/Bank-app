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
            migrationBuilder.AddColumn<string>(
                name: "ClienteCedula",
                table: "Cuentas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_ClienteCedula",
                table: "Cuentas",
                column: "ClienteCedula",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cuentas_Clientes_ClienteCedula",
                table: "Cuentas",
                column: "ClienteCedula",
                principalTable: "Clientes",
                principalColumn: "Cedula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cuentas_Clientes_ClienteCedula",
                table: "Cuentas");

            migrationBuilder.DropIndex(
                name: "IX_Cuentas_ClienteCedula",
                table: "Cuentas");

            migrationBuilder.DropColumn(
                name: "ClienteCedula",
                table: "Cuentas");
        }
    }
}
