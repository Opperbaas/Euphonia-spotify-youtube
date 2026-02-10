using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class EnsureYouTubeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add YouTube fields if they do not exist yet
            migrationBuilder.Sql(@"IF COL_LENGTH('muziek','youtubeVideoId') IS NULL BEGIN ALTER TABLE [muziek] ADD [youtubeVideoId] nvarchar(50) NULL END");
            migrationBuilder.Sql(@"IF COL_LENGTH('muziek','youtubeThumbnailUrl') IS NULL BEGIN ALTER TABLE [muziek] ADD [youtubeThumbnailUrl] nvarchar(512) NULL END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF COL_LENGTH('muziek','youtubeVideoId') IS NOT NULL BEGIN ALTER TABLE [muziek] DROP COLUMN [youtubeVideoId] END");
            migrationBuilder.Sql(@"IF COL_LENGTH('muziek','youtubeThumbnailUrl') IS NOT NULL BEGIN ALTER TABLE [muziek] DROP COLUMN [youtubeThumbnailUrl] END");
        }
    }
}

