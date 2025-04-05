using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomKultury.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWydarzenIIchKategorii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KategoriaId",
                table: "Wydarzenie",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Lokalizacja",
                table: "Wydarzenie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Opis",
                table: "Wydarzenie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Wydarzenie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Kategoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wydarzenie_KategoriaId",
                table: "Wydarzenie",
                column: "KategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wydarzenie_Kategoria_KategoriaId",
                table: "Wydarzenie",
                column: "KategoriaId",
                principalTable: "Kategoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wydarzenie_Kategoria_KategoriaId",
                table: "Wydarzenie");

            migrationBuilder.DropTable(
                name: "Kategoria");

            migrationBuilder.DropIndex(
                name: "IX_Wydarzenie_KategoriaId",
                table: "Wydarzenie");

            migrationBuilder.DropColumn(
                name: "KategoriaId",
                table: "Wydarzenie");

            migrationBuilder.DropColumn(
                name: "Lokalizacja",
                table: "Wydarzenie");

            migrationBuilder.DropColumn(
                name: "Opis",
                table: "Wydarzenie");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Wydarzenie");
        }
    }
}
