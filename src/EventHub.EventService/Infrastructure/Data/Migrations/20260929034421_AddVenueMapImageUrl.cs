using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.EventService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueMapImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VenueMapImageUrl",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VenueMapImageUrl",
                table: "Events");
        }
    }
}
