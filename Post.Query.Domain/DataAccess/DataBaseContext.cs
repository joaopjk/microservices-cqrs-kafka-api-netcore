using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.DataAccess
{
    public class DataBaseContext : DbContext
    {
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DataBaseContext(DbContextOptions options) : base(options) { }
    }
}
