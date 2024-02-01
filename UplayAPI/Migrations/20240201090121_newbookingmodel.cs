using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UplayAPI.Migrations
{
    /// <inheritdoc />
    public partial class newbookingmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purchase_date",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Activity",
                table: "Bookings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Purchase_date",
                table: "Bookings",
                type: "longtext",
                nullable: false);
        }
    }
}
