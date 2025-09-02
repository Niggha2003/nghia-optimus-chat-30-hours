using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate59 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "GroupSignalRs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GroupSignalRs");
        }
    }
}
