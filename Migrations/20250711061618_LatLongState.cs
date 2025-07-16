using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class LatLongState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Sahalar",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Sahalar",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Sahalar");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Sahalar");
        }
    }
}
