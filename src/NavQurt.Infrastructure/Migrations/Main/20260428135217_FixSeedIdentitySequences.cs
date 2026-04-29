using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NavQurt.Infrastructure.Migrations.Main
{
    /// <inheritdoc />
    public partial class FixSeedIdentitySequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('"PaymentMethods"', 'Id'),
                    GREATEST(
                        COALESCE((SELECT MAX("Id") FROM "PaymentMethods"), 0) + 1,
                        nextval(pg_get_serial_sequence('"PaymentMethods"', 'Id'))),
                    false);
                """);

            migrationBuilder.Sql(
                """
                SELECT setval(
                    pg_get_serial_sequence('"Warehouses"', 'Id'),
                    GREATEST(
                        COALESCE((SELECT MAX("Id") FROM "Warehouses"), 0) + 1,
                        nextval(pg_get_serial_sequence('"Warehouses"', 'Id'))),
                    false);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
