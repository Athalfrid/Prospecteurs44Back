using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prospecteurs44Back.Migrations
{
    /// <inheritdoc />
    public partial class ModificationAlerteSOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AlerteSOS",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "AlerteSOS");
        }
    }
}
