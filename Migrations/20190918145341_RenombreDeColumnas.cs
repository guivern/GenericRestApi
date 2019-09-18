using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiBase.Migrations
{
    public partial class RenombreDeColumnas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioEditor",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioEditor",
                table: "Values");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioEditor",
                table: "Values",
                newName: "IdUsuarioModificador");

            migrationBuilder.RenameColumn(
                name: "FechaEdicion",
                table: "Values",
                newName: "UltimaModificacion");

            migrationBuilder.RenameIndex(
                name: "IX_Values_IdUsuarioEditor",
                table: "Values",
                newName: "IX_Values_IdUsuarioModificador");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioEditor",
                table: "Usuarios",
                newName: "IdUsuarioModificador");

            migrationBuilder.RenameColumn(
                name: "FechaEdicion",
                table: "Usuarios",
                newName: "UltimaModificacion");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdUsuarioEditor",
                table: "Usuarios",
                newName: "IX_Usuarios_IdUsuarioModificador");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioModificador",
                table: "Usuarios",
                column: "IdUsuarioModificador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioModificador",
                table: "Values",
                column: "IdUsuarioModificador",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioModificador",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Usuarios_IdUsuarioModificador",
                table: "Values");

            migrationBuilder.RenameColumn(
                name: "UltimaModificacion",
                table: "Values",
                newName: "FechaEdicion");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioModificador",
                table: "Values",
                newName: "IdUsuarioEditor");

            migrationBuilder.RenameIndex(
                name: "IX_Values_IdUsuarioModificador",
                table: "Values",
                newName: "IX_Values_IdUsuarioEditor");

            migrationBuilder.RenameColumn(
                name: "UltimaModificacion",
                table: "Usuarios",
                newName: "FechaEdicion");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioModificador",
                table: "Usuarios",
                newName: "IdUsuarioEditor");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdUsuarioModificador",
                table: "Usuarios",
                newName: "IX_Usuarios_IdUsuarioEditor");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioEditor",
                table: "Usuarios",
                column: "IdUsuarioEditor",
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
    }
}
