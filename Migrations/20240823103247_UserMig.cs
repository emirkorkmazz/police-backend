using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Police.Migrations
{
    /// <inheritdoc />
    public partial class UserMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: false, defaultValue: "User"),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    HashedUserIdentityNo = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CompanyAddress = table.Column<string>(type: "text", nullable: true),
                    CompanyPhone = table.Column<string>(type: "text", nullable: true),
                    BanCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    BanReason = table.Column<string>(type: "text", nullable: true),
                    BanDuration = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    BanEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LicenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }

}
