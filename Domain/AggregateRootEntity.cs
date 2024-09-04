namespace Domain;

public record AggregateRootEntity : BaseEntity, IAggregateRoot, ISoftDelete, IHasCreationTime, IHasDeletionTime, IHasModificationTime
{
    public bool IsDeleted { get; private set; }
    public DateTime CreationTime { get; private set; } = DateTime.Now;
    public DateTime? DeletionTime { get; private set; }
    public DateTime? LastModificationTime { get; private set; }

    public virtual void SoftDelete()
    {
        IsDeleted = true;
        DeletionTime = DateTime.Now;
    }

    public void NotifyModified()
    {
        LastModificationTime = DateTime.Now;
    }
}
