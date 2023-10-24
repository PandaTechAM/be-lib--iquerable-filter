using System.ComponentModel.DataAnnotations;

namespace TestFilters.Controllers.Models;

public class Phrase
{
    public string Text { get; set; } = null!;
    public DateTime Date { get; set; }

    [Key]
    public int Id { get; set; }
}