using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Police.Migrations
{
    /// <inheritdoc />
    public partial class PoliciesCreatedByAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Policies",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Policies");
        }
    }
}
