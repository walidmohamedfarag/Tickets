using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema_Ticket.Migrations
{
    /// <inheritdoc />
    public partial class addproppublicidforimageinactorandcinemaandmovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "MovieSubImgs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Cinemas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "MovieSubImgs");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Cinemas");
        }
    }
}
