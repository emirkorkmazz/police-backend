using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Police.Migrations
{
    /// <inheritdoc />
    public partial class UserCompanyIdAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyIdsJson",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyIdsJson",
                table: "Users");
        }
    }
}
