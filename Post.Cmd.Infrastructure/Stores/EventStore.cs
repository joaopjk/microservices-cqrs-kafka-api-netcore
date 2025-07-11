﻿using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _repository;
        private readonly IEventProducer _producer;
        private readonly string _kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

        public EventStore(IEventStoreRepository repository, IEventProducer producer)
        {
            _repository = repository;
            _producer = producer;
        }

        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _repository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException("");

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = eventType,
                    EventDate = @event
                };

                await _repository.SaveAsync(eventModel);
                await _producer.ProduceAsync(_kafkaTopic, @event);
            }
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _repository.FindByAggregateId(aggregateId);

            if (eventStream == null || !eventStream.Any())
                throw new AggregateNotFoundException("Incorrect post ID provided");

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventDate).ToList();
        }
    }
}
