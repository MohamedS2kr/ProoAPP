using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class LastUpdateInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.CreateTable(
                name: "priceEstimatedPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    basePrice = table.Column<decimal>(type: "decimal", nullable: false),
                    shortDistanceLimit = table.Column<decimal>(type: "decimal", nullable: false),
                    shortDistancePrice = table.Column<decimal>(type: "decimal", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priceEstimatedPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "priceCategoryTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceEstimatedPlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priceCategoryTiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_priceCategoryTiers_priceEstimatedPlans_PriceEstimatedPlanId",
                        column: x => x.PriceEstimatedPlanId,
                        principalTable: "priceEstimatedPlans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "pricePerDistances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistanceLimit = table.Column<decimal>(type: "decimal", nullable: false),
                    PricePerKm = table.Column<decimal>(type: "decimal", nullable: false),
                    PriceCategoryTierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricePerDistances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pricePerDistances_priceCategoryTiers_PriceCategoryTierId",
                        column: x => x.PriceCategoryTierId,
                        principalTable: "priceCategoryTiers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_priceCategoryTiers_PriceEstimatedPlanId",
                table: "priceCategoryTiers",
                column: "PriceEstimatedPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_pricePerDistances_PriceCategoryTierId",
                table: "pricePerDistances",
                column: "PriceCategoryTierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pricePerDistances");

            migrationBuilder.DropTable(
                name: "priceCategoryTiers");

            migrationBuilder.DropTable(
                name: "priceEstimatedPlans");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId",
                unique: true);
        }
    }
}
