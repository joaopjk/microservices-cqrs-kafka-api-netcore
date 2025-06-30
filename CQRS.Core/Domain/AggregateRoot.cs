using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; protected set; }
        private readonly List<BaseEvent> _changes = new();
        public int Version { get; set; } = -1;
        public IEnumerable<BaseEvent> GetUncommittedChanges() => new List<BaseEvent>(_changes);
        public void MarkChangesAsCommitted() => _changes.Clear();

        private void ApplyChanges(BaseEvent @event, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null)
#pragma warning disable CA2208
                throw new ArgumentNullException(nameof(method), $"The apply method was not found in the aggregate for {@event.GetType().Name}!");
#pragma warning restore CA2208

            method.Invoke(this, new object[] { @event });

            if(isNew)
                _changes.Add(@event);
        }

        protected void RaiseEvent(BaseEvent @event) => ApplyChanges(@event, true);

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyChanges(@event, false);
            }
        }
    }
}
