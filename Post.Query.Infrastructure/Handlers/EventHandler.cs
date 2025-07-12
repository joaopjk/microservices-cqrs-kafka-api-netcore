﻿using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task On(PostCreatedEvent @event)
        {
            await _postRepository.CreateAsync(new PostEntity
            {
                PostId = @event.Id,
                Author = @event.Author,
                Message = @event.Message,
                DatePosted = @event.DatePosted
            });
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);
            if (post == null) return;

            post.Message = @event.Message;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id);
            if (post == null) return;

            post.Likes++;
            await _postRepository.UpdateAsync(post);
        }

        public async Task On(CommentAddedEvent @event)
        {
            await _commentRepository.CreateAsync(new CommentEntity
            {
                PostId = @event.Id,
                CommentId = @event.CommentId,
                CommentDate = @event.CommentDate,
                Comment = @event.Comment,
                Username = @event.Username,
                Edited = false
            });
        }

        public async Task On(CommentUpdateEvent @event)
        {
            var comment = await _commentRepository.GetByIdAsync(@event.CommentId);
            if (comment == null) return;

            comment.Comment = @event.Comment;
            comment.Edited = true;
            comment.CommentDate = @event.EditDate;

            await _commentRepository.UpdateAsync(comment);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await _commentRepository.DeleteAsync(@event.CommentId);
        }

        public async Task On(PostRemovedEvent @event)
        {
            await _postRepository.DeleteAsync(@event.Id);
        }
    }
}
