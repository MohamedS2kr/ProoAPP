using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class UpdateTablesForDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Vehicles",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Make",
                table: "Vehicles",
                newName: "VehicleLicenseIdFront");

            migrationBuilder.RenameColumn(
                name: "LicensePlate",
                table: "Vehicles",
                newName: "VehicleLicenseIdBack");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Vehicles",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "VehicleDetails",
                table: "Drivers",
                newName: "LicenseIdFront");

            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "Drivers",
                newName: "LicenseIdBack");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Vehicles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "AirConditional",
                table: "Vehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Colour",
                table: "Vehicles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiringDate",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPalet",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPassenger",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "YeareOfManufacuter",
                table: "Vehicles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Rides",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "RideId",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Ratings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "RideId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "RideId",
                table: "LocationHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LocationHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiringDate",
                table: "Drivers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AirConditional",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Colour",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ExpiringDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NumberOfPalet",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NumberOfPassenger",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "YeareOfManufacuter",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExpiringDate",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Vehicles",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "VehicleLicenseIdFront",
                table: "Vehicles",
                newName: "Make");

            migrationBuilder.RenameColumn(
                name: "VehicleLicenseIdBack",
                table: "Vehicles",
                newName: "LicensePlate");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Vehicles",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "LicenseIdFront",
                table: "Drivers",
                newName: "VehicleDetails");

            migrationBuilder.RenameColumn(
                name: "LicenseIdBack",
                table: "Drivers",
                newName: "LicenseNumber");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Vehicles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Rides",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "Ratings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Ratings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "RideId",
                table: "LocationHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LocationHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");
        }
    }
}
