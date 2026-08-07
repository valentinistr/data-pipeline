namespace Core.ServiceBus.Models;

public enum EventBusMessageStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}
