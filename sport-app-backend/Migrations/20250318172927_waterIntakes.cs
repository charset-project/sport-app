using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sport_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class waterIntakes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterInDay_Athletes_AthleteId",
                table: "WaterInDay");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterInTake_Athletes_AthleteId",
                table: "WaterInTake");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterInTake",
                table: "WaterInTake");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterInDay",
                table: "WaterInDay");

            migrationBuilder.RenameTable(
                name: "WaterInTake",
                newName: "WaterInTakes");

            migrationBuilder.RenameTable(
                name: "WaterInDay",
                newName: "WaterInDays");

            migrationBuilder.RenameIndex(
                name: "IX_WaterInTake_AthleteId",
                table: "WaterInTakes",
                newName: "IX_WaterInTakes_AthleteId");

            migrationBuilder.RenameIndex(
                name: "IX_WaterInDay_AthleteId",
                table: "WaterInDays",
                newName: "IX_WaterInDays_AthleteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterInTakes",
                table: "WaterInTakes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterInDays",
                table: "WaterInDays",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterInDays_Athletes_AthleteId",
                table: "WaterInDays",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterInTakes_Athletes_AthleteId",
                table: "WaterInTakes",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterInDays_Athletes_AthleteId",
                table: "WaterInDays");

            migrationBuilder.DropForeignKey(
                name: "FK_WaterInTakes_Athletes_AthleteId",
                table: "WaterInTakes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterInTakes",
                table: "WaterInTakes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterInDays",
                table: "WaterInDays");

            migrationBuilder.RenameTable(
                name: "WaterInTakes",
                newName: "WaterInTake");

            migrationBuilder.RenameTable(
                name: "WaterInDays",
                newName: "WaterInDay");

            migrationBuilder.RenameIndex(
                name: "IX_WaterInTakes_AthleteId",
                table: "WaterInTake",
                newName: "IX_WaterInTake_AthleteId");

            migrationBuilder.RenameIndex(
                name: "IX_WaterInDays_AthleteId",
                table: "WaterInDay",
                newName: "IX_WaterInDay_AthleteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterInTake",
                table: "WaterInTake",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterInDay",
                table: "WaterInDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterInDay_Athletes_AthleteId",
                table: "WaterInDay",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WaterInTake_Athletes_AthleteId",
                table: "WaterInTake",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
