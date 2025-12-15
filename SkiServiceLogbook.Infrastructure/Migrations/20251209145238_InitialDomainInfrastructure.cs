using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiServiceLogbook.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDomainInfrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkiPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PairNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Length = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkiPairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AirTemperature = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    SnowTemperature = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    Humidity = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    SnowCondition = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaxCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaxCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaxInstructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", nullable: false),
                    Conditions = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaxInstructions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkiPairId = table.Column<int>(type: "INTEGER", nullable: false),
                    Side = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skis_SkiPairs_SkiPairId",
                        column: x => x.SkiPairId,
                        principalTable: "SkiPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestEventId = table.Column<int>(type: "INTEGER", nullable: true),
                    TournamentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SkiPair1Id = table.Column<int>(type: "INTEGER", nullable: true),
                    SkiPair2Id = table.Column<int>(type: "INTEGER", nullable: true),
                    WinnerSkiPairId = table.Column<int>(type: "INTEGER", nullable: true),
                    NextMatchId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_SkiPairs_SkiPair1Id",
                        column: x => x.SkiPair1Id,
                        principalTable: "SkiPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_SkiPairs_SkiPair2Id",
                        column: x => x.SkiPair2Id,
                        principalTable: "SkiPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_SkiPairs_WinnerSkiPairId",
                        column: x => x.WinnerSkiPairId,
                        principalTable: "SkiPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_TestEvents_TestEventId",
                        column: x => x.TestEventId,
                        principalTable: "TestEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TestEventPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkiPairId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ranking = table.Column<int>(type: "INTEGER", nullable: true),
                    MeasuredResult = table.Column<decimal>(type: "TEXT", precision: 10, scale: 3, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEventPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestEventPairs_SkiPairs_SkiPairId",
                        column: x => x.SkiPairId,
                        principalTable: "SkiPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestEventPairs_TestEvents_TestEventId",
                        column: x => x.TestEventId,
                        principalTable: "TestEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Waxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WaxCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MinTemperature = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    MaxTemperature = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    MinHumidity = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    MaxHumidity = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    SnowCondition = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waxes_WaxCategories_WaxCategoryId",
                        column: x => x.WaxCategoryId,
                        principalTable: "WaxCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestEventPairWaxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestEventPairId = table.Column<int>(type: "INTEGER", nullable: false),
                    WaxId = table.Column<int>(type: "INTEGER", nullable: false),
                    LayerOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestEventPairWaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestEventPairWaxes_TestEventPairs_TestEventPairId",
                        column: x => x.TestEventPairId,
                        principalTable: "TestEventPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestEventPairWaxes_Waxes_WaxId",
                        column: x => x.WaxId,
                        principalTable: "Waxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SkiPair1Id",
                table: "Matches",
                column: "SkiPair1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SkiPair2Id",
                table: "Matches",
                column: "SkiPair2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TestEventId",
                table: "Matches",
                column: "TestEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TournamentName_Round_MatchNumber",
                table: "Matches",
                columns: new[] { "TournamentName", "Round", "MatchNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_WinnerSkiPairId",
                table: "Matches",
                column: "WinnerSkiPairId");

            migrationBuilder.CreateIndex(
                name: "IX_SkiPairs_PairNumber",
                table: "SkiPairs",
                column: "PairNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skis_SkiPairId_Side",
                table: "Skis",
                columns: new[] { "SkiPairId", "Side" });

            migrationBuilder.CreateIndex(
                name: "IX_TestEventPairs_SkiPairId",
                table: "TestEventPairs",
                column: "SkiPairId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEventPairs_TestEventId_SkiPairId",
                table: "TestEventPairs",
                columns: new[] { "TestEventId", "SkiPairId" });

            migrationBuilder.CreateIndex(
                name: "IX_TestEventPairWaxes_TestEventPairId_LayerOrder",
                table: "TestEventPairWaxes",
                columns: new[] { "TestEventPairId", "LayerOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TestEventPairWaxes_WaxId",
                table: "TestEventPairWaxes",
                column: "WaxId");

            migrationBuilder.CreateIndex(
                name: "IX_TestEvents_AirTemperature_Humidity",
                table: "TestEvents",
                columns: new[] { "AirTemperature", "Humidity" });

            migrationBuilder.CreateIndex(
                name: "IX_TestEvents_EventDate",
                table: "TestEvents",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaxCategories_Name",
                table: "WaxCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Waxes_WaxCategoryId_Name",
                table: "Waxes",
                columns: new[] { "WaxCategoryId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Skis");

            migrationBuilder.DropTable(
                name: "TestEventPairWaxes");

            migrationBuilder.DropTable(
                name: "WaxInstructions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TestEventPairs");

            migrationBuilder.DropTable(
                name: "Waxes");

            migrationBuilder.DropTable(
                name: "SkiPairs");

            migrationBuilder.DropTable(
                name: "TestEvents");

            migrationBuilder.DropTable(
                name: "WaxCategories");
        }
    }
}
