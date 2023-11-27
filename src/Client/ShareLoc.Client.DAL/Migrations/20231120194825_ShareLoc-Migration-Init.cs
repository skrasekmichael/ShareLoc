using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareLoc.Client.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ShareLocMigrationInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    LocalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    CratedUTC = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SharedUTC = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.LocalId);
                });

            migrationBuilder.CreateTable(
                name: "Guesses",
                columns: table => new
                {
                    LocalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LocalPlaceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GuesserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Distance = table.Column<double>(type: "REAL", nullable: false),
                    PlaceEntityLocalId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guesses", x => x.LocalId);
                    table.ForeignKey(
                        name: "FK_Guesses_Places_LocalPlaceId",
                        column: x => x.LocalPlaceId,
                        principalTable: "Places",
                        principalColumn: "LocalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guesses_Places_PlaceEntityLocalId",
                        column: x => x.PlaceEntityLocalId,
                        principalTable: "Places",
                        principalColumn: "LocalId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_LocalPlaceId",
                table: "Guesses",
                column: "LocalPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Guesses_PlaceEntityLocalId",
                table: "Guesses",
                column: "PlaceEntityLocalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guesses");

            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
