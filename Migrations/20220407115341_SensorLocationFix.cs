using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalApi.Migrations
{
    public partial class SensorLocationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorLocations_Temperatures_TemperatureId",
                table: "SensorLocations");

            migrationBuilder.RenameColumn(
                name: "TemperatureId",
                table: "SensorLocations",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_SensorLocations_TemperatureId",
                table: "SensorLocations",
                newName: "IX_SensorLocations_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorLocations_Locations_LocationId",
                table: "SensorLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorLocations_Locations_LocationId",
                table: "SensorLocations");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "SensorLocations",
                newName: "TemperatureId");

            migrationBuilder.RenameIndex(
                name: "IX_SensorLocations_LocationId",
                table: "SensorLocations",
                newName: "IX_SensorLocations_TemperatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorLocations_Temperatures_TemperatureId",
                table: "SensorLocations",
                column: "TemperatureId",
                principalTable: "Temperatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
