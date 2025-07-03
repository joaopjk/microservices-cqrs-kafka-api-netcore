using Microsoft.EntityFrameworkCore;

namespace Post.Query.Domain.DataAccess
{
    public class DataBaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;
        public DataBaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public DataBaseContext CreateDbContext()
        {
            DbContextOptionsBuilder<DataBaseContext> optionsBuilder = new();
            _configureDbContext(optionsBuilder);

            return new DataBaseContext(optionsBuilder.Options);
        }
    }
}
