using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiBase.Migrations
{
    public partial class RolesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Usuarios",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdRol",
                table: "Usuarios",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Usuarios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NroDocumento",
                table: "Usuarios",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Activo = table.Column<bool>(nullable: false),
                    Nombre = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Activo", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1L, true, "Posee todos los permisos", "Administrador" },
                    { 2L, true, "Posee permisos de solo lectura", "Operador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IdRol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "NroDocumento",
                table: "Usuarios");
        }
    }
}
