using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAIS.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_GenderCategories_GenderCategoryId",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_GenderCategoryId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "GenderCategoryId",
                table: "Applicants");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_GenderId",
                table: "Applicants",
                column: "GenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_GenderCategories_GenderId",
                table: "Applicants",
                column: "GenderId",
                principalTable: "GenderCategories",
                principalColumn: "GenderCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_GenderCategories_GenderId",
                table: "Applicants");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_GenderId",
                table: "Applicants");

            migrationBuilder.AddColumn<int>(
                name: "GenderCategoryId",
                table: "Applicants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_GenderCategoryId",
                table: "Applicants",
                column: "GenderCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_GenderCategories_GenderCategoryId",
                table: "Applicants",
                column: "GenderCategoryId",
                principalTable: "GenderCategories",
                principalColumn: "GenderCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
