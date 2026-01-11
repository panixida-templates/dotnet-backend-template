using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dal.Ef.Migrations
{
    /// <inheritdoc />
    public partial class _20260111_185344_Alter_Column_id_In_domain_users_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_domain_users",
                table: "domain_users");

            migrationBuilder.AddColumn<Guid>(
                name: "id_new",
                table: "domain_users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.DropColumn(
                name: "id",
                table: "domain_users");

            migrationBuilder.RenameColumn(
                name: "id_new",
                table: "domain_users",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_domain_users",
                table: "domain_users",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_domain_users",
                table: "domain_users");

            migrationBuilder.AddColumn<int>(
                name: "id_old",
                table: "domain_users",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.DropColumn(
                name: "id",
                table: "domain_users");

            migrationBuilder.RenameColumn(
                name: "id_old",
                table: "domain_users",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_domain_users",
                table: "domain_users",
                column: "id");
        }
    }
}
