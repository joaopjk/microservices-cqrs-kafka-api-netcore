using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Post.Query.Domain.DataAccess;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Repositories
{
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    public class PostRepository : IPostRepository
    {
        private readonly DataBaseContextFactory _dataBaseContextFactory;

        public PostRepository(DataBaseContextFactory dataBaseContextFactory)
        {
            _dataBaseContextFactory = dataBaseContextFactory;
        }

        public async Task CreateAsync(PostEntity post)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            context.Posts.Add(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            context.Posts.Update(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            var post = await GetByIdAsync(postId);

            if(post == null) return;

            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            return await context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(x => x.PostId == postId);
        }

        public async Task<List<PostEntity>> GetAllAsync()
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostEntity>> GetByAuthorAsync(string author)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .Where(x=>x.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<List<PostEntity>> GetListWithLikesAsync(int numberOfLikes)
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .Where(x => x.Likes >= numberOfLikes)
                .ToListAsync();
        }

        public async Task<List<PostEntity>> GetListWithCommentsAsync()
        {
            await using var context = _dataBaseContextFactory.CreateDbContext();
            return await context.Posts
                .AsNoTracking()
                .Include(p => p.Comments)
                .AsNoTracking()
                .Where(x => x.Comments != null && x.Comments.Any())
                .ToListAsync();
        }
    }
}
