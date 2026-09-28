using IPL_Franchises.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Tests.Helpers;

public static class TestDbContextFactory
{
    public static async Task<IPLDbContext> CreateAsync()
    {
        var options =
            new DbContextOptionsBuilder<IPLDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

        var context =
            new IPLDbContext(options);

        /*
         * SQLite in-memory databases exist only
         * while their connection remains open.
         */
        await context.Database.OpenConnectionAsync();

        /*
         * Creates the schema including indexes,
         * constraints and ASP.NET Identity tables.
         */
        await context.Database.EnsureCreatedAsync();

        return context;
    }
}
