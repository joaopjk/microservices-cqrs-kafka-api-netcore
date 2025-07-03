using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.DataAccess;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Post.Query.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataBaseContextFactory _dataBaseContextFactory;

        public CommentRepository(DataBaseContextFactory dataBaseContextFactory)
        {
            _dataBaseContextFactory = dataBaseContextFactory;
        }

        public async Task CreateAsync(CommentEntity comment)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            context.Comments.Add(comment);
            _ = await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            context.Comments.Update(comment);
            _ = await context.SaveChangesAsync();
        }

        public async Task<CommentEntity?> GetByIdAsync(Guid commentId)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            return await context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task DeleteAsync(Guid commentId)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            var comment = await GetByIdAsync(commentId);

            if(comment == null) return;

            context.Comments.Remove(comment);
            _ = await context.SaveChangesAsync();
        }
    }
}
