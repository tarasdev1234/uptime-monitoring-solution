namespace Uptime.Monitoring.Model.Models
{
    public enum EventType
    {
        EventTtlExpired = 0,
        Up = 1,
        Down = 2,
        Paused = 3,
        Started = 4,
        Stopped = 5
    }
}
