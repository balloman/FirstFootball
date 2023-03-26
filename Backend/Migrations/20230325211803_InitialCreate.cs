using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace FirstFootball.Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "fixtures",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    home_team_name = table.Column<string>(type: "text", nullable: false),
                    away_team_name = table.Column<string>(type: "text", nullable: false),
                    home_score = table.Column<int>(type: "integer", nullable: false),
                    away_score = table.Column<int>(type: "integer", nullable: false),
                    match_week = table.Column<int>(type: "integer", nullable: false),
                    date_posted = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fixtures", x => x.id);
                    table.ForeignKey(
                        name: "fk_fixtures_teams_away_team_temp_id",
                        column: x => x.away_team_name,
                        principalTable: "teams",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fixtures_teams_home_team_temp_id1",
                        column: x => x.home_team_name,
                        principalTable: "teams",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fixtures_away_team_name",
                table: "fixtures",
                column: "away_team_name");

            migrationBuilder.CreateIndex(
                name: "ix_fixtures_date_posted",
                table: "fixtures",
                column: "date_posted");

            migrationBuilder.CreateIndex(
                name: "ix_fixtures_home_team_name",
                table: "fixtures",
                column: "home_team_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fixtures");

            migrationBuilder.DropTable(
                name: "teams");
        }
    }
}
