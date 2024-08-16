using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasksMvc.Migrations
{
    /// <inheritdoc />
    public partial class AdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT Id FROM AspNetRoles WHERE Id = 'A3B31704-B29F-4693-9842-81664CA3E2C9')
BEGIN
	INSERT AspNetRoles (Id,[Name], [NormalizedName])
	VALUES ('A3B31704-B29F-4693-9842-81664CA3E2C9', 'Admin', 'ADMIN')
END

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles WHERE Id = 'A3B31704-B29F-4693-9842-81664CA3E2C9'");
        }
    }
}
