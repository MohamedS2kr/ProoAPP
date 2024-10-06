using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class AddTwoTableFotRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Passengers_PassengerId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_RideId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Ratings");

            migrationBuilder.AlterColumn<string>(
                name: "PassengerId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "DriverRating_DriverId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverRating_PassengerId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverRating_RideId1",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RideId1",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_DriverRating_DriverId",
                table: "Ratings",
                column: "DriverRating_DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_DriverRating_PassengerId",
                table: "Ratings",
                column: "DriverRating_PassengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_DriverRating_RideId1",
                table: "Ratings",
                column: "DriverRating_RideId1");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RideId",
                table: "Ratings",
                column: "RideId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RideId1",
                table: "Ratings",
                column: "RideId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Drivers_DriverRating_DriverId",
                table: "Ratings",
                column: "DriverRating_DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Passengers_DriverRating_PassengerId",
                table: "Ratings",
                column: "DriverRating_PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Passengers_PassengerId",
                table: "Ratings",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Rides_DriverRating_RideId1",
                table: "Ratings",
                column: "DriverRating_RideId1",
                principalTable: "Rides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Rides_RideId1",
                table: "Ratings",
                column: "RideId1",
                principalTable: "Rides",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Drivers_DriverRating_DriverId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Passengers_DriverRating_PassengerId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Passengers_PassengerId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Rides_DriverRating_RideId1",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Rides_RideId1",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_DriverRating_DriverId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_DriverRating_PassengerId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_DriverRating_RideId1",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_RideId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_RideId1",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "DriverRating_DriverId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "DriverRating_PassengerId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "DriverRating_RideId1",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "RideId1",
                table: "Ratings");

            migrationBuilder.AlterColumn<string>(
                name: "PassengerId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Ratings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RideId",
                table: "Ratings",
                column: "RideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Passengers_PassengerId",
                table: "Ratings",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
