using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vermundo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInfomaniakIdToSubscriber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "infomaniak_id",
                table: "newsletter_subscribers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "infomaniak_id",
                table: "newsletter_subscribers");
        }
    }
}
