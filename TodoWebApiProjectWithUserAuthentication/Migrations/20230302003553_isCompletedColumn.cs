using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoWebApiProjectWithUserAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class isCompletedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCompleted",
                table: "ListItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCompleted",
                table: "ListItem");
        }
    }
}
