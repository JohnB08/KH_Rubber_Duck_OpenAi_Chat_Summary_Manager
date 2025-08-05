using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubberDuckMCPServer.Migrations
{
    /// <inheritdoc />
    public partial class chatIdLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawJsons_Chats_ChatId",
                table: "RawJsons");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "RawJsons",
                newName: "UniqueChatId");

            migrationBuilder.RenameIndex(
                name: "IX_RawJsons_ChatId",
                table: "RawJsons",
                newName: "IX_RawJsons_UniqueChatId");

            migrationBuilder.AddColumn<int>(
                name: "UniqueChatId",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChatId",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GeneratedChatId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatId", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UniqueChatId",
                table: "Chats",
                column: "UniqueChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatId_UniqueChatId",
                table: "Chats",
                column: "UniqueChatId",
                principalTable: "ChatId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawJsons_ChatId_UniqueChatId",
                table: "RawJsons",
                column: "UniqueChatId",
                principalTable: "ChatId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatId_UniqueChatId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_RawJsons_ChatId_UniqueChatId",
                table: "RawJsons");

            migrationBuilder.DropTable(
                name: "ChatId");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UniqueChatId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UniqueChatId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "UniqueChatId",
                table: "RawJsons",
                newName: "ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_RawJsons_UniqueChatId",
                table: "RawJsons",
                newName: "IX_RawJsons_ChatId");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "Chats",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_RawJsons_Chats_ChatId",
                table: "RawJsons",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
