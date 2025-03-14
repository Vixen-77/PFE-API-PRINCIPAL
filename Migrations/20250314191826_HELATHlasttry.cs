using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIAPP.Migrations
{
    /// <inheritdoc />
    public partial class HELATHlasttry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityDiplome",
                table: "ProSs");

            migrationBuilder.DropColumn(
                name: "IdentityRecordPath",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalRecordPath",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsValidated",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValidated",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "IdentityDiplome",
                table: "ProSs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalRecordPath",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityRecordPath",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
