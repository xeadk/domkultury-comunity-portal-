using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomKultury.Migrations
{
    /// <inheritdoc />
    public partial class ZmienionaKategoriaID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wydarzenie_Kategoria_KategoriaId",
                table: "Wydarzenie");

            migrationBuilder.AlterColumn<int>(
                name: "KategoriaId",
                table: "Wydarzenie",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Wydarzenie_Kategoria_KategoriaId",
                table: "Wydarzenie",
                column: "KategoriaId",
                principalTable: "Kategoria",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wydarzenie_Kategoria_KategoriaId",
                table: "Wydarzenie");

            migrationBuilder.AlterColumn<int>(
                name: "KategoriaId",
                table: "Wydarzenie",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wydarzenie_Kategoria_KategoriaId",
                table: "Wydarzenie",
                column: "KategoriaId",
                principalTable: "Kategoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
