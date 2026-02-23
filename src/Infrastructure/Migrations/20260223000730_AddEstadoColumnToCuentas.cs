using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fast_Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoColumnToCuentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Cuentas",
                type: "TEXT",
                nullable: false,
                defaultValue: "Activa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Cuentas");
        }
    }
}
