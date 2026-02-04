using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageBack.Migrations
{
    /// <inheritdoc />
    public partial class UserPermissionTableRemoveIsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserPermissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
