using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSC_Stream.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUrlToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImage",
                table: "Episode");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Episode",
                newName: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "Episode",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "BannerImage",
                table: "Episode",
                type: "text",
                nullable: true);
        }
    }
}
