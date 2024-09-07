using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proo.Infrastructer.Data.Migrations
{
    public partial class addInDriver_PropertyStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Drivers");
        }
    }
}
