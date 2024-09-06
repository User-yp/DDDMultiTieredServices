using Domain;

namespace MTS.Domain.Entity;

public record Model:AggregateRootEntity, IAggregateRoot
{
    public string ModelName { get;private set; }
    public string ModelVersion { get;private set; }
    public string ModelDescription { get; private set; }
    public Model()
    {
        
    }
    public Model(string ModelName, string ModelVersion, string ModelDescription)
    {
        this.ModelName = ModelName;
        this.ModelVersion = ModelVersion;
        this.ModelDescription = ModelDescription;
    }

}