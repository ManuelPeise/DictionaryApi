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
                name: "ImportFileTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "longtext", nullable: false),
                    Key = table.Column<string>(type: "longtext", nullable: false),
                    SourceLanguage = table.Column<int>(type: "int", nullable: false),
                    Translations = table.Column<string>(type: "longtext", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FileBytes = table.Column<string>(type: "longtext", nullable: false),
                    IsImportedSuccessful = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportFileTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    ResourceKey = table.Column<string>(type: "longtext", nullable: false),
                    TranslationType = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
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
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
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
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
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
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
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
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
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
                name: "VocabularyCategoryTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GroupGuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    SourceLanguage = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyCategoryTable", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VocabularyTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GroupGuid = table.Column<Guid>(type: "char(36)", nullable: false),
                    Word = table.Column<string>(type: "longtext", nullable: false),
                    Article = table.Column<string>(type: "longtext", nullable: true),
                    ExampleSentence = table.Column<string>(type: "longtext", nullable: true),
                    Ipa = table.Column<string>(type: "longtext", nullable: true),
                    IsReviewRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    PartOfSpeechId = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyTable_LanguageTable_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LanguageTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyTable_PartOfSpeachTable_PartOfSpeechId",
                        column: x => x.PartOfSpeechId,
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
                    FirstName = table.Column<string>(type: "longtext", nullable: false),
                    LastName = table.Column<string>(type: "longtext", nullable: false),
                    EmailAddress = table.Column<string>(type: "longtext", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "longblob", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    UserCredentialsId = table.Column<int>(type: "int", nullable: false),
                    UserSettingsId = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
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
                name: "VocabularySessionTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SessionType = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularySessionTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularySessionTable_VocabularyCategoryTable_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "VocabularyCategoryTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VocabularyToCategoriesTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyToCategoriesTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyToCategoriesTable_VocabularyCategoryTable_Category~",
                        column: x => x.CategoryId,
                        principalTable: "VocabularyCategoryTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyToCategoriesTable_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VocabularyProgressTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TimesSeen = table.Column<int>(type: "int", nullable: false),
                    Success = table.Column<int>(type: "int", nullable: false),
                    Failed = table.Column<int>(type: "int", nullable: false),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    VocabularySessionEntityId = table.Column<int>(type: "int", nullable: true),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyProgressTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyProgressTable_VocabularySessionTable_VocabularySes~",
                        column: x => x.VocabularySessionEntityId,
                        principalTable: "VocabularySessionTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VocabularyProgressTable_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VocabularySessionResultTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    End = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Success = table.Column<int>(type: "int", nullable: false),
                    Failed = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    IsDirty = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdExternal = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularySessionResultTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularySessionResultTable_VocabularySessionTable_SessionId",
                        column: x => x.SessionId,
                        principalTable: "VocabularySessionTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VocabularySessionVocabulary",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    VocabularyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularySessionVocabulary", x => new { x.SessionId, x.VocabularyId });
                    table.ForeignKey(
                        name: "FK_VocabularySessionVocabulary_VocabularySessionTable_SessionId",
                        column: x => x.SessionId,
                        principalTable: "VocabularySessionTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularySessionVocabulary_VocabularyTable_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "VocabularyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "LanguageTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IdExternal", "IsDirty", "Name", "ResourceKey", "TranslationType", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("18fda2d3-3f6f-4963-8495-d498defbb54d"), false, "German", "LanguageGerman", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("16dd7950-90d0-4e80-a80b-a4268e65d736"), false, "English", "LanguageEnglish", 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("65ae65cb-3dff-4d98-9a62-42b2cd8c6140"), false, "Danish", "LanguageDanish", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" }
                });

            migrationBuilder.InsertData(
                table: "PartOfSpeachTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IdExternal", "IsDirty", "Name", "ResourceKey", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("e70f0216-1360-41af-9e4c-40f06574deb4"), false, "adjective", "PartOfSpeache70f0216-1360-41af-9e4c-40f06574deb4", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("132a81ea-c686-498e-b593-d6f871ffd6ea"), false, "adverb", "PartOfSpeach132a81ea-c686-498e-b593-d6f871ffd6ea", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("00783292-d4f8-439e-bb2a-8ced803f0e70"), false, "article", "PartOfSpeach00783292-d4f8-439e-bb2a-8ced803f0e70", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("3fd7b958-22fc-4389-9687-599b9d05d6d3"), false, "noun", "PartOfSpeach3fd7b958-22fc-4389-9687-599b9d05d6d3", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("3548d4e7-e5fe-4280-9362-40b6016569b4"), false, "verb", "PartOfSpeach3548d4e7-e5fe-4280-9362-40b6016569b4", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("06ebc0cd-7d23-4958-bb28-2b936f079359"), false, "preposition", "PartOfSpeach06ebc0cd-7d23-4958-bb28-2b936f079359", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("ebc23f3f-7e7c-4dec-a50d-6a32917e9b48"), false, "pronoun", "PartOfSpeachebc23f3f-7e7c-4dec-a50d-6a32917e9b48", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new Guid("7aac8e85-19ed-462e-8ef5-5a916c960133"), false, "proper noun", "PartOfSpeach7aac8e85-19ed-462e-8ef5-5a916c960133", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" }
                });

            migrationBuilder.InsertData(
                table: "UserCredentialsTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ExpireDate", "IdExternal", "IsDirty", "PasswordHash", "RefreshToken", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("01b8eb28-654b-4b34-9577-ad406be5b9c2"), false, "$2a$12$ZUdEbhrrKfyY2zomnIEXXOUVtIQ6J8VeWFk40bcGtceHfRG5cBlVC", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "" });

            migrationBuilder.InsertData(
                table: "UserSettingsTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Culture", "IdExternal", "IsAutoDataSyncEnabled", "IsDirty", "UpdatedAt", "UpdatedBy", "UseLocalDataStore" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", 0, new Guid("fade763a-f81c-4872-8e42-d31839406565"), true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false });

            migrationBuilder.InsertData(
                table: "UserTable",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "EmailAddress", "FirstName", "IdExternal", "IsDirty", "LastName", "ProfileImage", "UpdatedAt", "UpdatedBy", "UserCredentialsId", "UserRole", "UserSettingsId" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", new DateTime(1980, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin.user@app.com", "Admin", new Guid("4fd79b86-8f6a-4202-9e6a-f556deb04da9"), false, "User", new byte[0], new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 1, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserCredentialsId",
                table: "UserTable",
                column: "UserCredentialsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_UserSettingsId",
                table: "UserTable",
                column: "UserSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyProgressTable_VocabularyId",
                table: "VocabularyProgressTable",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyProgressTable_VocabularySessionEntityId",
                table: "VocabularyProgressTable",
                column: "VocabularySessionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularySessionResultTable_SessionId",
                table: "VocabularySessionResultTable",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularySessionTable_CategoryId",
                table: "VocabularySessionTable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularySessionVocabulary_VocabularyId",
                table: "VocabularySessionVocabulary",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTable_LanguageId",
                table: "VocabularyTable",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTable_PartOfSpeechId",
                table: "VocabularyTable",
                column: "PartOfSpeechId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyToCategoriesTable_CategoryId",
                table: "VocabularyToCategoriesTable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyToCategoriesTable_VocabularyId_CategoryId",
                table: "VocabularyToCategoriesTable",
                columns: new[] { "VocabularyId", "CategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportFileTable");

            migrationBuilder.DropTable(
                name: "LogMessageTable");

            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "VocabularyProgressTable");

            migrationBuilder.DropTable(
                name: "VocabularySessionResultTable");

            migrationBuilder.DropTable(
                name: "VocabularySessionVocabulary");

            migrationBuilder.DropTable(
                name: "VocabularyToCategoriesTable");

            migrationBuilder.DropTable(
                name: "UserCredentialsTable");

            migrationBuilder.DropTable(
                name: "UserSettingsTable");

            migrationBuilder.DropTable(
                name: "VocabularySessionTable");

            migrationBuilder.DropTable(
                name: "VocabularyTable");

            migrationBuilder.DropTable(
                name: "VocabularyCategoryTable");

            migrationBuilder.DropTable(
                name: "LanguageTable");

            migrationBuilder.DropTable(
                name: "PartOfSpeachTable");
        }
    }
}
