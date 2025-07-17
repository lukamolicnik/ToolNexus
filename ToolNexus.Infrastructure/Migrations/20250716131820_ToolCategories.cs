using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ToolCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tools");

            migrationBuilder.AddColumn<int>(
                name: "ToolCategoryId",
                table: "Tools",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ToolCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tools_ToolCategoryId",
                table: "Tools",
                column: "ToolCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolCategories_Code",
                table: "ToolCategories",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tools_ToolCategories_ToolCategoryId",
                table: "Tools",
                column: "ToolCategoryId",
                principalTable: "ToolCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tools_ToolCategories_ToolCategoryId",
                table: "Tools");

            migrationBuilder.DropTable(
                name: "ToolCategories");

            migrationBuilder.DropIndex(
                name: "IX_Tools_ToolCategoryId",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "ToolCategoryId",
                table: "Tools");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Tools",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
