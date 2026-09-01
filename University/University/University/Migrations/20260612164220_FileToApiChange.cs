using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Migrations
{
    /// <inheritdoc />
    public partial class FileToApiChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "FileToApis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "FileToApis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "FileToApis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FileToApis_CourseId",
                table: "FileToApis",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileToApis_Course_CourseId",
                table: "FileToApis",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileToApis_Course_CourseId",
                table: "FileToApis");

            migrationBuilder.DropIndex(
                name: "IX_FileToApis_CourseId",
                table: "FileToApis");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "FileToApis");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "FileToApis");

            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "FileToApis");
        }
    }
}
