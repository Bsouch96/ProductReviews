using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductReviews.Migrations
{
    public partial class CreateTableMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "_productReviews",
                schema: "ProductReviews",
                newName: "_productReviews");

            migrationBuilder.UpdateData(
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 1,
                column: "ProductReviewDate",
                value: new DateTime(2021, 12, 14, 16, 28, 21, 937, DateTimeKind.Local).AddTicks(2612));

            migrationBuilder.UpdateData(
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 2,
                column: "ProductReviewDate",
                value: new DateTime(2021, 12, 14, 16, 28, 21, 941, DateTimeKind.Local).AddTicks(2779));

            migrationBuilder.UpdateData(
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 3,
                column: "ProductReviewDate",
                value: new DateTime(2021, 12, 14, 16, 28, 21, 941, DateTimeKind.Local).AddTicks(2848));

            migrationBuilder.UpdateData(
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 4,
                column: "ProductReviewDate",
                value: new DateTime(2021, 12, 14, 16, 28, 21, 941, DateTimeKind.Local).AddTicks(2855));

            migrationBuilder.UpdateData(
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 5,
                column: "ProductReviewDate",
                value: new DateTime(2021, 12, 14, 16, 28, 21, 941, DateTimeKind.Local).AddTicks(2858));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProductReviews");

            migrationBuilder.RenameTable(
                name: "_productReviews",
                newName: "_productReviews",
                newSchema: "ProductReviews");

            migrationBuilder.UpdateData(
                schema: "ProductReviews",
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 1,
                column: "ProductReviewDate",
                value: new DateTime(2021, 11, 22, 16, 59, 24, 617, DateTimeKind.Local).AddTicks(1879));

            migrationBuilder.UpdateData(
                schema: "ProductReviews",
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 2,
                column: "ProductReviewDate",
                value: new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1629));

            migrationBuilder.UpdateData(
                schema: "ProductReviews",
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 3,
                column: "ProductReviewDate",
                value: new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1680));

            migrationBuilder.UpdateData(
                schema: "ProductReviews",
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 4,
                column: "ProductReviewDate",
                value: new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1686));

            migrationBuilder.UpdateData(
                schema: "ProductReviews",
                table: "_productReviews",
                keyColumn: "ProductReviewID",
                keyValue: 5,
                column: "ProductReviewDate",
                value: new DateTime(2021, 11, 22, 16, 59, 24, 622, DateTimeKind.Local).AddTicks(1690));
        }
    }
}
