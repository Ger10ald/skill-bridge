// SkillBridge.Tests/Data/EfTestHelpers.cs
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SkillBridge.Tests.Data
{
    public sealed class DbScope<TContext> : IDisposable, IAsyncDisposable where TContext : DbContext
    {
        public SqliteConnection Connection { get; }
        public TContext Context { get; }

        internal DbScope(SqliteConnection connection, TContext context)
        {
            Connection = connection;
            Context = context;
        }

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }

        public void Deconstruct(out SqliteConnection conn, out TContext ctx)
        {
            conn = Connection;
            ctx = Context;
        }
    }

    public static class EfTestHelpers
    {
        public static DbScope<TContext> CreateSqliteInMemory<TContext>(
            Func<DbContextOptions<TContext>, TContext> factory)
            where TContext : DbContext
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();

            var options = new DbContextOptionsBuilder<TContext>()
                .UseSqlite(conn)
                .EnableSensitiveDataLogging()
                .Options;

            var ctx = factory(options);
            ctx.Database.EnsureCreated();

            return new DbScope<TContext>(conn, ctx);
        }
    }
}
