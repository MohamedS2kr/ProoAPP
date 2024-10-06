using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class FinalRowUpjdateInRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverRatings_Passengers_PassengerId",
                table: "DriverRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverRatings_Rides_RideId1",
                table: "DriverRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Drivers_DriverId",
                table: "PassengerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Passengers_PassengerId",
                table: "PassengerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Rides_RideId1",
                table: "PassengerRatings");

            migrationBuilder.DropIndex(
                name: "IX_PassengerRatings_DriverId",
                table: "PassengerRatings");

            migrationBuilder.DropIndex(
                name: "IX_PassengerRatings_RideId",
                table: "PassengerRatings");

            migrationBuilder.DropIndex(
                name: "IX_PassengerRatings_RideId1",
                table: "PassengerRatings");

            migrationBuilder.DropIndex(
                name: "IX_DriverRatings_PassengerId",
                table: "DriverRatings");

            migrationBuilder.DropIndex(
                name: "IX_DriverRatings_RideId",
                table: "DriverRatings");

            migrationBuilder.DropIndex(
                name: "IX_DriverRatings_RideId1",
                table: "DriverRatings");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "PassengerRatings");

            migrationBuilder.DropColumn(
                name: "RideId1",
                table: "PassengerRatings");

            migrationBuilder.DropColumn(
                name: "PassengerId",
                table: "DriverRatings");

            migrationBuilder.DropColumn(
                name: "RideId1",
                table: "DriverRatings");

            migrationBuilder.AlterColumn<string>(
                name: "PassengerId",
                table: "PassengerRatings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "DriverRatings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassengerRatings_RideId",
                table: "PassengerRatings",
                column: "RideId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRatings_RideId",
                table: "DriverRatings",
                column: "RideId");

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerRatings_Passengers_PassengerId",
                table: "PassengerRatings",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Passengers_PassengerId",
                table: "PassengerRatings");

            migrationBuilder.DropIndex(
                name: "IX_PassengerRatings_RideId",
                table: "PassengerRatings");

            migrationBuilder.DropIndex(
                name: "IX_DriverRatings_RideId",
                table: "DriverRatings");

            migrationBuilder.AlterColumn<string>(
                name: "PassengerId",
                table: "PassengerRatings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "PassengerRatings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RideId1",
                table: "PassengerRatings",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "DriverRatings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "PassengerId",
                table: "DriverRatings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RideId1",
                table: "DriverRatings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassengerRatings_DriverId",
                table: "PassengerRatings",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_PassengerRatings_RideId",
                table: "PassengerRatings",
                column: "RideId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PassengerRatings_RideId1",
                table: "PassengerRatings",
                column: "RideId1");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRatings_PassengerId",
                table: "DriverRatings",
                column: "PassengerId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRatings_RideId",
                table: "DriverRatings",
                column: "RideId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DriverRatings_RideId1",
                table: "DriverRatings",
                column: "RideId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverRatings_Passengers_PassengerId",
                table: "DriverRatings",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverRatings_Rides_RideId1",
                table: "DriverRatings",
                column: "RideId1",
                principalTable: "Rides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerRatings_Drivers_DriverId",
                table: "PassengerRatings",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerRatings_Passengers_PassengerId",
                table: "PassengerRatings",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerRatings_Rides_RideId1",
                table: "PassengerRatings",
                column: "RideId1",
                principalTable: "Rides",
                principalColumn: "Id");
        }
    }
}
