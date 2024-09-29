using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class addCategoryOfVehicleAndUpdateNamingInDriverModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Vehicles",
                newName: "VehicleModel");

            migrationBuilder.RenameColumn(
                name: "NumberOfPalet",
                table: "Vehicles",
                newName: "NumberOfPlate");

            migrationBuilder.RenameColumn(
                name: "ExpiringDate",
                table: "Vehicles",
                newName: "ExpiringDateOfVehicleLicence");

            migrationBuilder.RenameColumn(
                name: "LicenseIdFront",
                table: "Drivers",
                newName: "NationalIdFront");

            migrationBuilder.RenameColumn(
                name: "LicenseIdBack",
                table: "Drivers",
                newName: "NationalIdBack");

            migrationBuilder.RenameColumn(
                name: "ExpiringDate",
                table: "Drivers",
                newName: "DrivingLicenseExpiringDate");

            migrationBuilder.AddColumn<int>(
                name: "CategoryOfVehicleId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseIdBack",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseIdFront",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CategoryOfVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryOfVehicles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CategoryOfVehicleId",
                table: "Vehicles",
                column: "CategoryOfVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_CategoryOfVehicles_CategoryOfVehicleId",
                table: "Vehicles",
                column: "CategoryOfVehicleId",
                principalTable: "CategoryOfVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_CategoryOfVehicles_CategoryOfVehicleId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "CategoryOfVehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CategoryOfVehicleId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CategoryOfVehicleId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseIdBack",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseIdFront",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "VehicleModel",
                table: "Vehicles",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "NumberOfPlate",
                table: "Vehicles",
                newName: "NumberOfPalet");

            migrationBuilder.RenameColumn(
                name: "ExpiringDateOfVehicleLicence",
                table: "Vehicles",
                newName: "ExpiringDate");

            migrationBuilder.RenameColumn(
                name: "NationalIdFront",
                table: "Drivers",
                newName: "LicenseIdFront");

            migrationBuilder.RenameColumn(
                name: "NationalIdBack",
                table: "Drivers",
                newName: "LicenseIdBack");

            migrationBuilder.RenameColumn(
                name: "DrivingLicenseExpiringDate",
                table: "Drivers",
                newName: "ExpiringDate");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Vehicles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
