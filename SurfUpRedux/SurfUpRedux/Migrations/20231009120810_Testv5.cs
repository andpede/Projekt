using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfUpRedux.Migrations
{
    /// <inheritdoc />
    public partial class Testv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_BoardId",
                table: "Booking");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Board",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BoardId",
                table: "Booking",
                column: "BoardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Booking_BoardId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Board");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BoardId",
                table: "Booking",
                column: "BoardId");
        }
    }
}
