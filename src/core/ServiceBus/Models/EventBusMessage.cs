namespace Server.ServiceBus.Models;

public class EventBusMessage
{
    public int Id { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public EventBusMessageStatus Status { get; set; }
    public DateTime? CompletedTimestamp { get; set; }
}
