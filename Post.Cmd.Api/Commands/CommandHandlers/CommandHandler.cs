﻿using CQRS.Core.Handlers;
using Post.Cmd.Api.Commands.Interfaces;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.CommandHandlers
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

        public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task HandleAsync(NewPostCommand command)
        {
            var aggregate = new PostAggregate(command.Id, command.Author, command.Message);

            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditMessageCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetAsync(command.Id);
            aggregate.EditMessage(command.Message);

            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(LikePostCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetAsync(command.Id);
            aggregate.LikePost();

            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(AddCommentCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetAsync(command.Id);
            aggregate.AddComment(command.Comment, command.Username);

            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(EditCommentCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetAsync(command.Id);
            aggregate.EditComment(command.Id, command.Comment,command.Username);

            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(RemoveCommentCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetAsync(command.Id);
            aggregate.RemoveComment(command.Id, command.Username);

            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(DeletePostCommand command)
        {
            var aggregate = await _eventSourcingHandler.GetAsync(command.Id);
            aggregate.DeletePost(command.Username);

            await _eventSourcingHandler.SaveAsync(aggregate);
        }
    }
}
