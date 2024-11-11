using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeTrackingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToAnimeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MangaAuthor",
                table: "Animes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Animes",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Animes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MangaAuthor",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Animes");
        }
    }
}
