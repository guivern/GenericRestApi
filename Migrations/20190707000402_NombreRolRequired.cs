using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiBase.Migrations
{
    public partial class NombreRolRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Roles",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Roles",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
