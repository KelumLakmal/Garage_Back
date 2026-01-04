using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageBack.Migrations
{
    /// <inheritdoc />
    public partial class VehicleTableAddmgaePathColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Vehicles");
        }
    }
}
