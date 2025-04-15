using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomKultury.Migrations
{
    /// <inheritdoc />
    public partial class ScaloneKonteksty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instruktor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruktor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uczestnik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumerTelefonu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataRejestracji = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uczestnik", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zajecie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Termin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lokalizacja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaksymalnaLiczbaUczestnikow = table.Column<int>(type: "int", nullable: false),
                    InstruktorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zajecie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zajecie_Instruktor_InstruktorId",
                        column: x => x.InstruktorId,
                        principalTable: "Instruktor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UczestnikZajecie",
                columns: table => new
                {
                    UczestnikId = table.Column<int>(type: "int", nullable: false),
                    ZajecieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UczestnikZajecie", x => new { x.UczestnikId, x.ZajecieId });
                    table.ForeignKey(
                        name: "FK_UczestnikZajecie_Uczestnik_UczestnikId",
                        column: x => x.UczestnikId,
                        principalTable: "Uczestnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UczestnikZajecie_Zajecie_ZajecieId",
                        column: x => x.ZajecieId,
                        principalTable: "Zajecie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UczestnikZajecie_ZajecieId",
                table: "UczestnikZajecie",
                column: "ZajecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Zajecie_InstruktorId",
                table: "Zajecie",
                column: "InstruktorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UczestnikZajecie");

            migrationBuilder.DropTable(
                name: "Uczestnik");

            migrationBuilder.DropTable(
                name: "Zajecie");

            migrationBuilder.DropTable(
                name: "Instruktor");
        }
    }
}
