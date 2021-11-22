using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductReviews.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProductReviews");

            migrationBuilder.CreateTable(
                name: "_productReviews",
                schema: "ProductReviews",
                columns: table => new
                {
                    ProductReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductReviewHeader = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductReviewContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    ProductReviewIsHidden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__productReviews", x => x.ProductReviewID);
                });

            migrationBuilder.InsertData(
                schema: "ProductReviews",
                table: "_productReviews",
                columns: new[] { "ProductReviewID", "ProductID", "ProductReviewContent", "ProductReviewDate", "ProductReviewHeader", "ProductReviewIsHidden" },
                values: new object[,]
                {
                    { 1, 1, "Lovely Shoes.", new DateTime(2021, 11, 22, 16, 59, 24, 617, DateTimeKind.Local).AddTicks(1879), "Wow!", false },
                    { 2, 1, "Best shirt since sliced bread.", new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1629), "Amazing!", false },
                    { 3, 2, "Did not receive order...", new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1680), "Terrible!", false },
                    { 4, 2, "Great Service.", new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1686), "Lovely Jubbly!", false },
                    { 5, 3, "I wanted a schmedium but I received a Large. I'm so mad.", new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1690), "WrongSize!!!!", false }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_productReviews",
                schema: "ProductReviews");
        }
    }
}
