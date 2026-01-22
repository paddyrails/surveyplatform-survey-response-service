using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "survey_response");

            migrationBuilder.CreateTable(
                name: "responses",
                schema: "survey_response",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    survey_id = table.Column<Guid>(type: "uuid", nullable: false),
                    respondent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    respondent_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_anonymous = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_responses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "answers",
                schema: "survey_response",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    response_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text_value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    numeric_value = table.Column<int>(type: "integer", nullable: true),
                    boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                    date_value = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    selected_options = table.Column<string>(type: "jsonb", nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    scale_value = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers", x => x.id);
                    table.ForeignKey(
                        name: "FK_answers_responses_response_id",
                        column: x => x.response_id,
                        principalSchema: "survey_response",
                        principalTable: "responses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_answers_question_id",
                schema: "survey_response",
                table: "answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_answers_response_id",
                schema: "survey_response",
                table: "answers",
                column: "response_id");

            migrationBuilder.CreateIndex(
                name: "IX_answers_response_id_question_id",
                schema: "survey_response",
                table: "answers",
                columns: new[] { "response_id", "question_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_responses_respondent_id",
                schema: "survey_response",
                table: "responses",
                column: "respondent_id");

            migrationBuilder.CreateIndex(
                name: "IX_responses_status",
                schema: "survey_response",
                table: "responses",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_responses_survey_id",
                schema: "survey_response",
                table: "responses",
                column: "survey_id");

            migrationBuilder.CreateIndex(
                name: "IX_responses_survey_id_respondent_id",
                schema: "survey_response",
                table: "responses",
                columns: new[] { "survey_id", "respondent_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answers",
                schema: "survey_response");

            migrationBuilder.DropTable(
                name: "responses",
                schema: "survey_response");
        }
    }
}
