using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiBase.Migrations
{
    public partial class FixColumnNameUsario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsarioCreador",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsarioEditor",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Usuarios_IdUsarioCreador",
                table: "Values");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Usuarios_IdUsarioEditor",
                table: "Values");

            migrationBuilder.RenameColumn(
                name: "IdUsarioEditor",
                table: "Values",
                newName: "IdUsuarioEditor");

            migrationBuilder.RenameColumn(
                name: "IdUsarioCreador",
                table: "Values",
                newName: "IdUsuarioCreador");

            migrationBuilder.RenameIndex(
                name: "IX_Values_IdUsarioEditor",
                table: "Values",
                newName: "IX_Values_IdUsuarioEditor");

            migrationBuilder.RenameIndex(
                name: "IX_Values_IdUsarioCreador",
                table: "Values",
                newName: "IX_Values_IdUsuarioCreador");

            migrationBuilder.RenameColumn(
                name: "IdUsarioEditor",
                table: "Usuarios",
                newName: "IdUsuarioEditor");

            migrationBuilder.RenameColumn(
                name: "IdUsarioCreador",
                table: "Usuarios",
                newName: "IdUsuarioCreador");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdUsarioEditor",
                table: "Usuarios",
                newName: "IX_Usuarios_IdUsuarioEditor");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdUsarioCreador",
                table: "Usuarios",
                newName: "IX_Usuarios_IdUsuarioCreador");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioCreador",
                table: "Usuarios",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioEditor",
                table: "Usuarios",
                column: "IdUsuarioEditor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioCreador",
                table: "Values",
                column: "IdUsuarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioEditor",
                table: "Values",
                column: "IdUsuarioEditor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioCreador",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioEditor",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioCreador",
                table: "Values");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioEditor",
                table: "Values");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioEditor",
                table: "Values",
                newName: "IdUsarioEditor");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioCreador",
                table: "Values",
                newName: "IdUsarioCreador");

            migrationBuilder.RenameIndex(
                name: "IX_Values_IdUsuarioEditor",
                table: "Values",
                newName: "IX_Values_IdUsarioEditor");

            migrationBuilder.RenameIndex(
                name: "IX_Values_IdUsuarioCreador",
                table: "Values",
                newName: "IX_Values_IdUsarioCreador");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioEditor",
                table: "Usuarios",
                newName: "IdUsarioEditor");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioCreador",
                table: "Usuarios",
                newName: "IdUsarioCreador");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdUsuarioEditor",
                table: "Usuarios",
                newName: "IX_Usuarios_IdUsarioEditor");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdUsuarioCreador",
                table: "Usuarios",
                newName: "IX_Usuarios_IdUsarioCreador");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsarioCreador",
                table: "Usuarios",
                column: "IdUsarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsarioEditor",
                table: "Usuarios",
                column: "IdUsarioEditor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Usuarios_IdUsarioCreador",
                table: "Values",
                column: "IdUsarioCreador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Usuarios_IdUsarioEditor",
                table: "Values",
                column: "IdUsarioEditor",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
