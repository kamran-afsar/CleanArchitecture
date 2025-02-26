public abstract class DomainEvent
{
    protected DomainEvent()
    {
        DateOccurred = DateTime.UtcNow;
    }
    
    public bool IsPublished { get; set; }
    public DateTime DateOccurred { get; protected set; }
} 