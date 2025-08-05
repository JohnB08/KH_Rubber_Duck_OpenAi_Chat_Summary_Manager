using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubberDuckMCPServer.Migrations
{
    /// <inheritdoc />
    public partial class ensureChatIdTableCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatId_UniqueChatId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_RawJsons_ChatId_UniqueChatId",
                table: "RawJsons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatId",
                table: "ChatId");

            migrationBuilder.RenameTable(
                name: "ChatId",
                newName: "ChatIds");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatIds",
                table: "ChatIds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatIds_UniqueChatId",
                table: "Chats",
                column: "UniqueChatId",
                principalTable: "ChatIds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RawJsons_ChatIds_UniqueChatId",
                table: "RawJsons",
                column: "UniqueChatId",
                principalTable: "ChatIds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatIds_UniqueChatId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_RawJsons_ChatIds_UniqueChatId",
                table: "RawJsons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatIds",
                table: "ChatIds");

            migrationBuilder.RenameTable(
                name: "ChatIds",
                newName: "ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatId",
                table: "ChatId",
                column: "Id");

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
    }
}
