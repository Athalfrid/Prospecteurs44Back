using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Prospecteurs44Back.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    EventDescription = table.Column<string>(type: "text", nullable: false),
                    EventDateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventDateModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventStatus = table.Column<string>(type: "text", nullable: false),
                    TypeSortie = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "InformationsPersonnelles",
                columns: table => new
                {
                    InformationsPersonnellesID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Prenom = table.Column<string>(type: "text", nullable: false),
                    DateNaissance = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ville = table.Column<string>(type: "text", nullable: false),
                    CodePostal = table.Column<string>(type: "text", nullable: false),
                    Telephone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformationsPersonnelles", x => x.InformationsPersonnellesID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserPseudo = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    UserParrainUserId = table.Column<int>(type: "integer", nullable: true),
                    DateInscription = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InformationsPersonnellesID = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId");
                    table.ForeignKey(
                        name: "FK_Users_InformationsPersonnelles_InformationsPersonnellesID",
                        column: x => x.InformationsPersonnellesID,
                        principalTable: "InformationsPersonnelles",
                        principalColumn: "InformationsPersonnellesID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_UserParrainUserId",
                        column: x => x.UserParrainUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EventId",
                table: "Users",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InformationsPersonnellesID",
                table: "Users",
                column: "InformationsPersonnellesID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserParrainUserId",
                table: "Users",
                column: "UserParrainUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "InformationsPersonnelles");
        }
    }
}
