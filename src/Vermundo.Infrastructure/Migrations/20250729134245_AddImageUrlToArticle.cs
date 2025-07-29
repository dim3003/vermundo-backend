using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vermundo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "articles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "articles");
        }
    }
}
