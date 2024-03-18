using System.Text.Json.Serialization;

namespace TestFilters.db;

public class OneToMany
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; }
    
    [JsonIgnore]
    public Company Company { get; set; }
}