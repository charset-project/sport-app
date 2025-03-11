using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sport_app_backend.Migrations
{
    /// <inheritdoc />
    public partial class add_coach_questionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_CoachQuestion_CoachQuestionId",
                table: "Coaches");

            migrationBuilder.DropForeignKey(
                name: "FK_CoachQuestion_AspNetUsers_UserId",
                table: "CoachQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoachQuestion",
                table: "CoachQuestion");

            migrationBuilder.RenameTable(
                name: "CoachQuestion",
                newName: "CoachQuestions");

            migrationBuilder.RenameIndex(
                name: "IX_CoachQuestion_UserId",
                table: "CoachQuestions",
                newName: "IX_CoachQuestions_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "DifficultTrackAthletes",
                table: "CoachQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Disciplines",
                table: "CoachQuestions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "HardCommunicationWithAthletes",
                table: "CoachQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ManagingRevenue",
                table: "CoachQuestions",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motivations",
                table: "CoachQuestions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PresentsPracticeProgram",
                table: "CoachQuestions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "TrackAthlete",
                table: "CoachQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WorkOnlineWithAthletes",
                table: "CoachQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoachQuestions",
                table: "CoachQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_CoachQuestions_CoachQuestionId",
                table: "Coaches",
                column: "CoachQuestionId",
                principalTable: "CoachQuestions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoachQuestions_AspNetUsers_UserId",
                table: "CoachQuestions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_CoachQuestions_CoachQuestionId",
                table: "Coaches");

            migrationBuilder.DropForeignKey(
                name: "FK_CoachQuestions_AspNetUsers_UserId",
                table: "CoachQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoachQuestions",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "DifficultTrackAthletes",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "Disciplines",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "HardCommunicationWithAthletes",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "ManagingRevenue",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "Motivations",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "PresentsPracticeProgram",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "TrackAthlete",
                table: "CoachQuestions");

            migrationBuilder.DropColumn(
                name: "WorkOnlineWithAthletes",
                table: "CoachQuestions");

            migrationBuilder.RenameTable(
                name: "CoachQuestions",
                newName: "CoachQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_CoachQuestions_UserId",
                table: "CoachQuestion",
                newName: "IX_CoachQuestion_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoachQuestion",
                table: "CoachQuestion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_CoachQuestion_CoachQuestionId",
                table: "Coaches",
                column: "CoachQuestionId",
                principalTable: "CoachQuestion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoachQuestion_AspNetUsers_UserId",
                table: "CoachQuestion",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
