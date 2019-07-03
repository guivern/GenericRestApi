using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RestApiBase.Migrations
{
    public partial class ValuesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Activo = table.Column<bool>(nullable: false),
                    IdUsarioCreador = table.Column<long>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    IdUsarioEditor = table.Column<long>(nullable: true),
                    FechaEdicion = table.Column<DateTime>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Values_Usuarios_IdUsarioCreador",
                        column: x => x.IdUsarioCreador,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Values_Usuarios_IdUsarioEditor",
                        column: x => x.IdUsarioEditor,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Values_IdUsarioCreador",
                table: "Values",
                column: "IdUsarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Values_IdUsarioEditor",
                table: "Values",
                column: "IdUsarioEditor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
