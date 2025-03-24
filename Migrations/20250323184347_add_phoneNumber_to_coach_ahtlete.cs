using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sport_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class add_phoneNumber_to_coach_ahtlete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeightEntry_Athletes_AthleteId",
                table: "WeightEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeightEntry",
                table: "WeightEntry");

            migrationBuilder.RenameTable(
                name: "WeightEntry",
                newName: "WeightEntries");

            migrationBuilder.RenameColumn(
                name: "EntryDate",
                table: "WeightEntries",
                newName: "CurrentDate");

            migrationBuilder.RenameIndex(
                name: "IX_WeightEntry_AthleteId",
                table: "WeightEntries",
                newName: "IX_WeightEntries_AthleteId");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Coaches",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "WeightGoal",
                table: "Athletes",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "CurrentWeight",
                table: "Athletes",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Athletes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "WeightEntries",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeightEntries",
                table: "WeightEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeightEntries_Athletes_AthleteId",
                table: "WeightEntries",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeightEntries_Athletes_AthleteId",
                table: "WeightEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeightEntries",
                table: "WeightEntries");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Athletes");

            migrationBuilder.RenameTable(
                name: "WeightEntries",
                newName: "WeightEntry");

            migrationBuilder.RenameColumn(
                name: "CurrentDate",
                table: "WeightEntry",
                newName: "EntryDate");

            migrationBuilder.RenameIndex(
                name: "IX_WeightEntries_AthleteId",
                table: "WeightEntry",
                newName: "IX_WeightEntry_AthleteId");

            migrationBuilder.AlterColumn<int>(
                name: "WeightGoal",
                table: "Athletes",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentWeight",
                table: "Athletes",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "WeightEntry",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeightEntry",
                table: "WeightEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeightEntry_Athletes_AthleteId",
                table: "WeightEntry",
                column: "AthleteId",
                principalTable: "Athletes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
