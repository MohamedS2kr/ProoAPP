using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Migrations
{
    public partial class AddTableRideRequsetInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActualDistance",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ActualTime",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RideRequestsId",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RideRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupLatitude = table.Column<double>(type: "float", nullable: false),
                    PickupLongitude = table.Column<double>(type: "float", nullable: false),
                    DropoffAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DropoffLatitude = table.Column<double>(type: "float", nullable: false),
                    DropoffLongitude = table.Column<double>(type: "float", nullable: false),
                    EstimatedDistance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedTime = table.Column<int>(type: "int", nullable: false),
                    EstimatedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PassengerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RideRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RideRequests_Passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passengers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rides_RideRequestsId",
                table: "Rides",
                column: "RideRequestsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RideRequests_PassengerId",
                table: "RideRequests",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_RideRequests_RideRequestsId",
                table: "Rides",
                column: "RideRequestsId",
                principalTable: "RideRequests",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_RideRequests_RideRequestsId",
                table: "Rides");

            migrationBuilder.DropTable(
                name: "RideRequests");

            migrationBuilder.DropIndex(
                name: "IX_Rides_RideRequestsId",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ActualDistance",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ActualTime",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RideRequestsId",
                table: "Rides");
        }
    }
}
