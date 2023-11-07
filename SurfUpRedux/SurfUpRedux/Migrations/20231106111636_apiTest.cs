using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfUpRedux.Migrations
{
    /// <inheritdoc />
    public partial class apiTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardId1",
                table: "Booking",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BoardId1",
                table: "Booking",
                column: "BoardId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Board_BoardId1",
                table: "Booking",
                column: "BoardId1",
                principalTable: "Board",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Board_BoardId1",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_BoardId1",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BoardId1",
                table: "Booking");
        }
    }
}
