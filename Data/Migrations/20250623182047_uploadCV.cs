using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppJobSearchOnline.Data.Migrations
{
    /// <inheritdoc />
    public partial class uploadCV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVFileName",
                table: "JobApplication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CVFilePath",
                table: "JobApplication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CVFileType",
                table: "JobApplication",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVFileName",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "CVFilePath",
                table: "JobApplication");

            migrationBuilder.DropColumn(
                name: "CVFileType",
                table: "JobApplication");
        }
    }
}
