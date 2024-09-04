namespace Domain;
public interface IHasModificationTime
{
    DateTime? LastModificationTime { get; }
}