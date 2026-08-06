namespace Server.ServiceBus.Events;

public sealed class LogEvent
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
