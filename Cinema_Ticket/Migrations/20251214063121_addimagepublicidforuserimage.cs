using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema_Ticket.Migrations
{
    /// <inheritdoc />
    public partial class addimagepublicidforuserimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgPublicId",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgPublicId",
                table: "Actors");
        }
    }
}
