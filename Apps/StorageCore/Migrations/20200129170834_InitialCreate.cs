using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TheBall.Core.StorageCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlobStorageItems",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    ContentMD5 = table.Column<string>(nullable: true),
                    ETag = table.Column<string>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlobStorageItems", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlobStorageItems_ContentMD5",
                table: "BlobStorageItems",
                column: "ContentMD5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlobStorageItems");
        }
    }
}
