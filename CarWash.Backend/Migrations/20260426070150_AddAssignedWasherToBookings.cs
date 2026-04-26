using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWash.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedWasherToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedWasherId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AssignedWasherId",
                table: "Bookings",
                column: "AssignedWasherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_AssignedWasherId",
                table: "Bookings",
                column: "AssignedWasherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_AssignedWasherId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_AssignedWasherId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AssignedWasherId",
                table: "Bookings");
        }
    }
}
