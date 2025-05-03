using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomKultury.Migrations
{
    /// <inheritdoc />
    public partial class DodanieObrazkow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObrazekUrl",
                table: "Zajecie",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObrazekUrl",
                table: "Wydarzenie",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObrazekUrl",
                table: "Zajecie");

            migrationBuilder.DropColumn(
                name: "ObrazekUrl",
                table: "Wydarzenie");
        }
    }
}
