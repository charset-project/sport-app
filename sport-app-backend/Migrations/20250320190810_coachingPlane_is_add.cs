using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sport_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class coachingPlane_is_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coachplan_Coaches_CoachId",
                table: "Coachplan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Coachplan",
                table: "Coachplan");

            migrationBuilder.RenameTable(
                name: "Coachplan",
                newName: "CoachesPlan");

            migrationBuilder.RenameIndex(
                name: "IX_Coachplan_CoachId",
                table: "CoachesPlan",
                newName: "IX_CoachesPlan_CoachId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoachesPlan",
                table: "CoachesPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoachesPlan_Coaches_CoachId",
                table: "CoachesPlan",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoachesPlan_Coaches_CoachId",
                table: "CoachesPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoachesPlan",
                table: "CoachesPlan");

            migrationBuilder.RenameTable(
                name: "CoachesPlan",
                newName: "Coachplan");

            migrationBuilder.RenameIndex(
                name: "IX_CoachesPlan_CoachId",
                table: "Coachplan",
                newName: "IX_Coachplan_CoachId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coachplan",
                table: "Coachplan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coachplan_Coaches_CoachId",
                table: "Coachplan",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
