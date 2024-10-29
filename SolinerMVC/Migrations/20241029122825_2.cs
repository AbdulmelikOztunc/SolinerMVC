using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolinerMVC.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weather_Parameters_ParameterId",
                table: "Weather");

            migrationBuilder.DropForeignKey(
                name: "FK_Weather_Regions_RegionId",
                table: "Weather");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weather",
                table: "Weather");

            migrationBuilder.RenameTable(
                name: "Weather",
                newName: "Weathers");

            migrationBuilder.RenameIndex(
                name: "IX_Weather_RegionId_ParameterId_DateTime",
                table: "Weathers",
                newName: "IX_Weathers_RegionId_ParameterId_DateTime");

            migrationBuilder.RenameIndex(
                name: "IX_Weather_ParameterId",
                table: "Weathers",
                newName: "IX_Weathers_ParameterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weathers",
                table: "Weathers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weathers_Parameters_ParameterId",
                table: "Weathers",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weathers_Regions_RegionId",
                table: "Weathers",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weathers_Parameters_ParameterId",
                table: "Weathers");

            migrationBuilder.DropForeignKey(
                name: "FK_Weathers_Regions_RegionId",
                table: "Weathers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Weathers",
                table: "Weathers");

            migrationBuilder.RenameTable(
                name: "Weathers",
                newName: "Weather");

            migrationBuilder.RenameIndex(
                name: "IX_Weathers_RegionId_ParameterId_DateTime",
                table: "Weather",
                newName: "IX_Weather_RegionId_ParameterId_DateTime");

            migrationBuilder.RenameIndex(
                name: "IX_Weathers_ParameterId",
                table: "Weather",
                newName: "IX_Weather_ParameterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Weather",
                table: "Weather",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Weather_Parameters_ParameterId",
                table: "Weather",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Weather_Regions_RegionId",
                table: "Weather",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
