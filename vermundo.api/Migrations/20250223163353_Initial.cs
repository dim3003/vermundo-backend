using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vermundo.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleBulletLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ListIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleBulletLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleBulletPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BulletListId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleBulletPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleBulletPoints_ArticleBulletLists_BulletListId",
                        column: x => x.BulletListId,
                        principalTable: "ArticleBulletLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleParagraphs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ParagraphIndex = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleParagraphs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleParagraphs_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParagraphId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionInParagraph = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleImages_ArticleParagraphs_ParagraphId",
                        column: x => x.ParagraphId,
                        principalTable: "ArticleParagraphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleBulletPoints_BulletListId",
                table: "ArticleBulletPoints",
                column: "BulletListId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleImages_ParagraphId",
                table: "ArticleImages",
                column: "ParagraphId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleParagraphs_ArticleId",
                table: "ArticleParagraphs",
                column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleBulletPoints");

            migrationBuilder.DropTable(
                name: "ArticleImages");

            migrationBuilder.DropTable(
                name: "ArticleBulletLists");

            migrationBuilder.DropTable(
                name: "ArticleParagraphs");

            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
