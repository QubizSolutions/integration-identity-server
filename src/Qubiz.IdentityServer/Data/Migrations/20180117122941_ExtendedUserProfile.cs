using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Qubiz.IdentityServer.Data.Migrations
{
    public partial class ExtendedUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarRegistration",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractStartDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IDExpiryDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "IDIssueDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassExpiryDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PassIssueDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SkypeID",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TShirtSize",
                table: "UserProfiles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkStartDate",
                table: "UserProfiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CarRegistration",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ContractStartDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IDExpiryDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IDIssueDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PassExpiryDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PassIssueDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SkypeID",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "TShirtSize",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "WorkStartDate",
                table: "UserProfiles");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);
        }
    }
}
