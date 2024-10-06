using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class AddTwoTableFotRating02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_Ratings_Rides_RideId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Rides_RideId1",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
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

            migrationBuilder.DropColumn(
                name: "Discriminator",
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

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "PassengerRatings");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_RideId1",
                table: "PassengerRatings",
                newName: "IX_PassengerRatings_RideId1");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_RideId",
                table: "PassengerRatings",
                newName: "IX_PassengerRatings_RideId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_PassengerId",
                table: "PassengerRatings",
                newName: "IX_PassengerRatings_PassengerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_DriverId",
                table: "PassengerRatings",
                newName: "IX_PassengerRatings_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PassengerRatings",
                table: "PassengerRatings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DriverRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PassengerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RideId = table.Column<int>(type: "int", nullable: false),
                    RideId1 = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverRatings_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverRatings_Passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passengers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverRatings_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverRatings_Rides_RideId1",
                        column: x => x.RideId1,
                        principalTable: "Rides",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverRatings_DriverId",
                table: "DriverRatings",
                column: "DriverId");

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
                name: "FK_PassengerRatings_Rides_RideId",
                table: "PassengerRatings",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PassengerRatings_Rides_RideId1",
                table: "PassengerRatings",
                column: "RideId1",
                principalTable: "Rides",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Drivers_DriverId",
                table: "PassengerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Passengers_PassengerId",
                table: "PassengerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Rides_RideId",
                table: "PassengerRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PassengerRatings_Rides_RideId1",
                table: "PassengerRatings");

            migrationBuilder.DropTable(
                name: "DriverRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PassengerRatings",
                table: "PassengerRatings");

            migrationBuilder.RenameTable(
                name: "PassengerRatings",
                newName: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_PassengerRatings_RideId1",
                table: "Ratings",
                newName: "IX_Ratings_RideId1");

            migrationBuilder.RenameIndex(
                name: "IX_PassengerRatings_RideId",
                table: "Ratings",
                newName: "IX_Ratings_RideId");

            migrationBuilder.RenameIndex(
                name: "IX_PassengerRatings_PassengerId",
                table: "Ratings",
                newName: "IX_Ratings_PassengerId");

            migrationBuilder.RenameIndex(
                name: "IX_PassengerRatings_DriverId",
                table: "Ratings",
                newName: "IX_Ratings_DriverId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Ratings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

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
                name: "FK_Ratings_Rides_RideId",
                table: "Ratings",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Rides_RideId1",
                table: "Ratings",
                column: "RideId1",
                principalTable: "Rides",
                principalColumn: "Id");
        }
    }
}
