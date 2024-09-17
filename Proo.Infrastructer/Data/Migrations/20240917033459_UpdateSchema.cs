using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class UpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationHistories");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Rides",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "PickupLocation",
                table: "Rides",
                newName: "PickupLocation_Address");

            migrationBuilder.RenameColumn(
                name: "Fare",
                table: "Rides",
                newName: "FarePrice");

            migrationBuilder.RenameColumn(
                name: "DropoffLocation",
                table: "Rides",
                newName: "DestinationLocation_Address");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Rides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Rides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DestinationLocation_Latitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DestinationLocation_Longitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLocation_Latitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PickupLocation_Longitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DestinationLocation_Latitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DestinationLocation_Longitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupLocation_Latitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PickupLocation_Longitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "PickupLocation_Address",
                table: "Rides",
                newName: "PickupLocation");

            migrationBuilder.RenameColumn(
                name: "FarePrice",
                table: "Rides",
                newName: "Fare");

            migrationBuilder.RenameColumn(
                name: "DestinationLocation_Address",
                table: "Rides",
                newName: "DropoffLocation");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Rides",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "LocationHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RideId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationHistories_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationHistories_RideId",
                table: "LocationHistories",
                column: "RideId");
        }
    }
}
