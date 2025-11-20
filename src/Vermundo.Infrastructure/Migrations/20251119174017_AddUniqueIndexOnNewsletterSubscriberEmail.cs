using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vermundo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexOnNewsletterSubscriberEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "newsletter_subscribers",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_newsletter_subscribers_email",
                table: "newsletter_subscribers",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_newsletter_subscribers_email",
                table: "newsletter_subscribers");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "newsletter_subscribers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(320)",
                oldMaxLength: 320);
        }
    }
}
