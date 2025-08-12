using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DBRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustedBy",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "TableName",
                table: "AuditTrails");

            migrationBuilder.RenameColumn(
                name: "AdjustedAt",
                table: "StockAdjustments",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StockAdjustments_AdjustedAt",
                table: "StockAdjustments",
                newName: "IX_StockAdjustments_CreatedAt");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "Tools",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tools",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Tools",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "ToolCategories",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "ToolCategories",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "Suppliers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Suppliers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "StockAdjustments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "DeliveryNotes",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "DeliveryNotes",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "AuditTrails",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Tools_CreatedBy",
                table: "Tools",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tools_UpdatedBy",
                table: "Tools",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ToolCategories_CreatedBy",
                table: "ToolCategories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ToolCategories_UpdatedBy",
                table: "ToolCategories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CreatedBy",
                table: "Suppliers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_UpdatedBy",
                table: "Suppliers",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_CreatedBy",
                table: "StockAdjustments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNotes_CreatedBy",
                table: "DeliveryNotes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryNotes_UpdatedBy",
                table: "DeliveryNotes",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditTrails_Users_UserId",
                table: "AuditTrails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryNotes_Users_CreatedBy",
                table: "DeliveryNotes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryNotes_Users_UpdatedBy",
                table: "DeliveryNotes",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_Users_CreatedBy",
                table: "StockAdjustments",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Users_CreatedBy",
                table: "Suppliers",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Users_UpdatedBy",
                table: "Suppliers",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ToolCategories_Users_CreatedBy",
                table: "ToolCategories",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ToolCategories_Users_UpdatedBy",
                table: "ToolCategories",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tools_Users_CreatedBy",
                table: "Tools",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tools_Users_UpdatedBy",
                table: "Tools",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditTrails_Users_UserId",
                table: "AuditTrails");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryNotes_Users_CreatedBy",
                table: "DeliveryNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryNotes_Users_UpdatedBy",
                table: "DeliveryNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_Users_CreatedBy",
                table: "StockAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Users_CreatedBy",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Users_UpdatedBy",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_ToolCategories_Users_CreatedBy",
                table: "ToolCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ToolCategories_Users_UpdatedBy",
                table: "ToolCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tools_Users_CreatedBy",
                table: "Tools");

            migrationBuilder.DropForeignKey(
                name: "FK_Tools_Users_UpdatedBy",
                table: "Tools");

            migrationBuilder.DropIndex(
                name: "IX_Tools_CreatedBy",
                table: "Tools");

            migrationBuilder.DropIndex(
                name: "IX_Tools_UpdatedBy",
                table: "Tools");

            migrationBuilder.DropIndex(
                name: "IX_ToolCategories_CreatedBy",
                table: "ToolCategories");

            migrationBuilder.DropIndex(
                name: "IX_ToolCategories_UpdatedBy",
                table: "ToolCategories");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_CreatedBy",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_UpdatedBy",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_CreatedBy",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryNotes_CreatedBy",
                table: "DeliveryNotes");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryNotes_UpdatedBy",
                table: "DeliveryNotes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockAdjustments");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "StockAdjustments",
                newName: "AdjustedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StockAdjustments_CreatedAt",
                table: "StockAdjustments",
                newName: "IX_StockAdjustments_AdjustedAt");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Tools",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tools",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Tools",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ToolCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ToolCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Suppliers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Suppliers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdjustedBy",
                table: "StockAdjustments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "DeliveryNotes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "DeliveryNotes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AuditTrails",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TableName",
                table: "AuditTrails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
