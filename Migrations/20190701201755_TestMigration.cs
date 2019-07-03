using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiBase.Migrations
{
    public partial class TestMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Activo = table.Column<bool>(nullable: false),
                    IdUsarioCreador = table.Column<long>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    IdUsarioEditor = table.Column<long>(nullable: true),
                    FechaEdicion = table.Column<DateTime>(nullable: true),
                    Username = table.Column<string>(maxLength: 12, nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Usuarios_IdUsarioCreador",
                        column: x => x.IdUsarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Usuarios_IdUsarioEditor",
                        column: x => x.IdUsarioEditor,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdUsarioCreador",
                table: "Usuarios",
                column: "IdUsarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdUsarioEditor",
                table: "Usuarios",
                column: "IdUsarioEditor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
