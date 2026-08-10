namespace Core.EventBus.Store;

public sealed class InMemoryEventStore
{
    private readonly object _gate = new();
    private readonly List<object> _events = [];

    public void Enqueue(object @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        lock (_gate)
        {
            _events.Add(@event);
        }
    }

    public IReadOnlyList<TEvent> DequeueAll<TEvent>() where TEvent : class
    {
        lock (_gate)
        {
            var matching = new List<TEvent>();
            for (var i = 0; i < _events.Count;)
            {
                if (_events[i] is TEvent typed)
                {
                    matching.Add(typed);
                    _events.RemoveAt(i);
                    continue;
                }

                i++;
            }

            return matching;
        }
    }
}
