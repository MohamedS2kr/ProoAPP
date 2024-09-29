using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class addedTheModelsAndTypesOfVehicles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_CategoryOfVehicles_CategoryOfVehicleId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Drivers_DriverId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CategoryOfVehicleId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "CategoryOfVehicleId",
                table: "Vehicles",
                newName: "VehicleModelId");

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryOfVehicleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTypes_CategoryOfVehicles_CategoryOfVehicleId",
                        column: x => x.CategoryOfVehicleId,
                        principalTable: "CategoryOfVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleTypeId",
                table: "VehicleModels",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_CategoryOfVehicleId",
                table: "VehicleTypes",
                column: "CategoryOfVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_DriverId",
                table: "Vehicles",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Drivers_DriverId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "VehicleModelId",
                table: "Vehicles",
                newName: "CategoryOfVehicleId");

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                table: "Vehicles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_DriverId",
                table: "Vehicles",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
