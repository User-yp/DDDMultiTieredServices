namespace Elasticsearch.Domain.Entity;

public class Actors : BaseIndex
{
    public DateTime RegistrationDate { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime BirthDate { get; set; }
    public int Age { get; set; }
    public int TotalMovies { get; set; }
    public string Movies { get; set; }
}
