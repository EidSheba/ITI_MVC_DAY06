using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("6b08246b-cfe1-4878-88f3-52c1d0455f83"));

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: new Guid("6f2bad93-c5b2-4b88-9adf-3e07d501b709"));

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "Id", "Bio", "FirstName", "IsActive", "LastName", "Specialization" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "Experienced software developer and instructor", "John", true, "Doe", 0 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Category", "Description", "EndDate", "InstructorId", "IsActive", "Name", "StartDate" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), 0, "Learn the fundamentals of C# programming", new DateTime(2025, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), true, "Introduction to C#", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "Id", "Bio", "FirstName", "IsActive", "LastName", "Specialization" },
                values: new object[] { new Guid("6f2bad93-c5b2-4b88-9adf-3e07d501b709"), "Experienced software developer and instructor", "John", true, "Doe", 0 });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Category", "Description", "EndDate", "InstructorId", "IsActive", "Name", "StartDate" },
                values: new object[] { new Guid("6b08246b-cfe1-4878-88f3-52c1d0455f83"), 0, "Learn the fundamentals of C# programming", new DateTime(2025, 9, 23, 15, 58, 36, 652, DateTimeKind.Local).AddTicks(2091), new Guid("6f2bad93-c5b2-4b88-9adf-3e07d501b709"), true, "Introduction to C#", new DateTime(2025, 8, 24, 15, 58, 36, 648, DateTimeKind.Local).AddTicks(2898) });
        }
    }
}
