using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "order",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "order",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "model",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "model",
                newName: "CreationTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "model",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "order");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "model");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                table: "order",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "order",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                table: "model",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "model",
                newName: "CreateTime");
        }
    }
}
