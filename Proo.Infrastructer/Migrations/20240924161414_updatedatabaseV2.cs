using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Migrations
{
    public partial class updatedatabaseV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Rides_RideId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Passengers_PassengerId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_RideRequests_RideRequestsId",
                table: "Rides");

            migrationBuilder.DropIndex(
                name: "IX_Rides_RideRequestsId",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ActualDistance",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "Rides",
                newName: "LastModifiedAt");

            migrationBuilder.RenameColumn(
                name: "ActualTime",
                table: "Rides",
                newName: "paymentMethod");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Ratings",
                newName: "PassengerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                newName: "IX_Ratings_PassengerId");

            migrationBuilder.RenameColumn(
                name: "RideId",
                table: "Bids",
                newName: "RideRequestsId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_RideId",
                table: "Bids",
                newName: "IX_Bids_RideRequestsId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Rides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RideRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "RideRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "RideRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "paymentMethod",
                table: "RideRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Review",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DriverId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveTime",
                table: "Drivers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "LastLat",
                table: "Drivers",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastLng",
                table: "Drivers",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StatusWork",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOtpValid",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Rides_RideRequestsId",
                table: "Rides",
                column: "RideRequestsId");

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_DriverId",
                table: "RideRequests",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_DriverId",
                table: "Ratings",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_RideRequests_RideRequestsId",
                table: "Bids",
                column: "RideRequestsId",
                principalTable: "RideRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Drivers_DriverId",
                table: "RideRequests",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Passengers_PassengerId",
                table: "RideRequests",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_RideRequests_RideRequestsId",
                table: "Rides",
                column: "RideRequestsId",
                principalTable: "RideRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_RideRequests_RideRequestsId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Drivers_DriverId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Passengers_PassengerId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Drivers_DriverId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RideRequests_Passengers_PassengerId",
                table: "RideRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Rides_RideRequests_RideRequestsId",
                table: "Rides");

            migrationBuilder.DropIndex(
                name: "IX_Rides_RideRequestsId",
                table: "Rides");

            migrationBuilder.DropIndex(
                name: "IX_RideRequests_DriverId",
                table: "RideRequests");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_DriverId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "paymentMethod",
                table: "RideRequests");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "LastActiveTime",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LastLat",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LastLng",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "StatusWork",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "IsOtpValid",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "paymentMethod",
                table: "Rides",
                newName: "ActualTime");

            migrationBuilder.RenameColumn(
                name: "LastModifiedAt",
                table: "Rides",
                newName: "AssignedAt");

            migrationBuilder.RenameColumn(
                name: "PassengerId",
                table: "Ratings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_PassengerId",
                table: "Ratings",
                newName: "IX_Ratings_UserId");

            migrationBuilder.RenameColumn(
                name: "RideRequestsId",
                table: "Bids",
                newName: "RideId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_RideRequestsId",
                table: "Bids",
                newName: "IX_Bids_RideId");

            migrationBuilder.AddColumn<double>(
                name: "ActualDistance",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Review",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rides_RideRequestsId",
                table: "Rides",
                column: "RideRequestsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Rides_RideId",
                table: "Bids",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RideRequests_Passengers_PassengerId",
                table: "RideRequests",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_RideRequests_RideRequestsId",
                table: "Rides",
                column: "RideRequestsId",
                principalTable: "RideRequests",
                principalColumn: "Id");
        }
    }
}
