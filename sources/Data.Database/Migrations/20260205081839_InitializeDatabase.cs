using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    ResourceKey = table.Column<string>(type: "longtext", nullable: false),
                    LanguageType = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LogMessageTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(type: "longtext", nullable: false),
                    ExeptionMessage = table.Column<string>(type: "longtext", nullable: true),
                    StackTrace = table.Column<string>(type: "longtext", nullable: true),
                    Module = table.Column<string>(type: "longtext", nullable: false),
                    LogMessageType = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogMessageTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PartOfSpeachTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    ResourceKey = table.Column<string>(type: "longtext", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartOfSpeachTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserCredentialsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentialsTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserSettingsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Culture = table.Column<int>(type: "int", nullable: false),
                    UseLocalDataStore = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsAutoDataSyncEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettingsTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VocabularyTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    VocabularyGuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    Topic = table.Column<string>(type: "longtext", nullable: false),
                    PartOfSpeachId = table.Column<int>(type: "int", nullable: false),
                    LanguageEntityId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyTable_LanguageTable_LanguageEntityId",
                        column: x => x.LanguageEntityId,
                        principalTable: "LanguageTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VocabularyTable_PartOfSpeachTable_PartOfSpeachId",
                        column: x => x.PartOfSpeachId,
                        principalTable: "PartOfSpeachTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserIdExternal = table.Column<string>(type: "longtext", nullable: false),
                    FirstName = table.Column<string>(type: "longtext", nullable: false),
                    LastName = table.Column<string>(type: "longtext", nullable: false),
                    EmailAddress = table.Column<string>(type: "longtext", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "longblob", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    UserCredentialsId = table.Column<int>(type: "int", nullable: false),
                    UserSettingsId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTable_UserCredentialsTable_UserCredentialsId",
                        column: x => x.UserCredentialsId,
                        principalTable: "UserCredentialsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTable_UserSettingsTable_UserSettingsId",
                        column: x => x.UserSettingsId,
                        principalTable: "UserSettingsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TranslationTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    VocabularyGuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    Word = table.Column<string>(type: "longtext", nullable: false),
                    Ipa = table.Column<string>(type: "longtext", nullable: false),
                    Sentence = table.Column<string>(type: "longtext", nullable: false),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Synonyms = table.Column<string>(type: "longtext", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationTable_LanguageTable_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LanguageTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationTable_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "LanguageTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "LanguageType", "Name", "ResourceKey", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 2, "German", "LanguageGerman", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 0, "English", "LanguageEnglish", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 1, "Danish", "LanguageDanish", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" }
                });

            migrationBuilder.InsertData(
                table: "PartOfSpeachTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Name", "ResourceKey", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "adjective", "PartOfSpeachadjective", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "adverb", "PartOfSpeachadverb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "article", "PartOfSpeacharticle", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "noun", "PartOfSpeachnoun", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "verb", "PartOfSpeachverb", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "preposition", "PartOfSpeachpreposition", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "pronoun", "PartOfSpeachpronoun", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "proper noun", "PartOfSpeachproper noun", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" }
                });

            migrationBuilder.InsertData(
                table: "UserCredentialsTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ExpireDate", "PasswordHash", "RefreshToken", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$12$ZUdEbhrrKfyY2zomnIEXXOUVtIQ6J8VeWFk40bcGtceHfRG5cBlVC", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" });

            migrationBuilder.InsertData(
                table: "UserSettingsTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Culture", "IsAutoDataSyncEnabled", "UpdatedAt", "UpdatedBy", "UseLocalDataStore" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 0, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false });

            migrationBuilder.InsertData(
                table: "UserTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "EmailAddress", "FirstName", "LastName", "ProfileImage", "UpdatedAt", "UpdatedBy", "UserCredentialsId", "UserIdExternal", "UserRole", "UserSettingsId" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new DateTime(1980, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin.user@app.com", "Admin", "User", new byte[0], new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 1, "95f6f461-b8eb-48f8-aecf-78bbd0cc0c3d", 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_TranslationTable_LanguageId",
                table: "TranslationTable",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationTable_VocabularyId",
                table: "TranslationTable",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserCredentialsId",
                table: "UserTable",
                column: "UserCredentialsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserSettingsId",
                table: "UserTable",
                column: "UserSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTable_LanguageEntityId",
                table: "VocabularyTable",
                column: "LanguageEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTable_PartOfSpeachId",
                table: "VocabularyTable",
                column: "PartOfSpeachId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogMessageTable");

            migrationBuilder.DropTable(
                name: "TranslationTable");

            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "VocabularyTable");

            migrationBuilder.DropTable(
                name: "UserCredentialsTable");

            migrationBuilder.DropTable(
                name: "UserSettingsTable");

            migrationBuilder.DropTable(
                name: "LanguageTable");

            migrationBuilder.DropTable(
                name: "PartOfSpeachTable");
        }
    }
}
