using CQRS.Core.Domain;
using CQRS.Core.Events;
using MongoDB.Driver;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository()
        {
            var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGO_DB_CONNECTION_STRING"));
            var mongoDatabase = mongoClient.GetDatabase(Environment.GetEnvironmentVariable("DATABASE"));

            _eventStoreCollection =
                mongoDatabase.GetCollection<EventModel>(Environment.GetEnvironmentVariable("COLLECTION"));
        }
        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }

        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
